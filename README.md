# DataProcessor

A .NET library for processing and formatting data records with customizable formatting options.

## Overview

DataProcessor is a flexible .NET 9.0 library that provides components for processing data records, formatting decimal values with different rounding options, and outputting data to CSV files or TCP payloads. It now includes a powerful flexible message format system that allows for client-specific message formats.

## Features

- Support for different rounding modes (Truncate, RoundUp, RoundDown)
- Configurable decimal precision (0-3 decimal places)
- CSV file creation with customizable formatting
- TCP payload generation for network transmission
- **NEW**: Flexible message format system with client-specific formats
- **NEW**: Dictionary-based data records with flexible schema
- Extensible architecture using interfaces and dependency injection

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later

### Installation

Clone the repository and build the solution:
git clone https://github.com/yourusername/DataProcessor.git
cd DataProcessor
dotnet build
### Running Tests
dotnet test
## Usage Examples

### Basic Usage
// Create formatter with 0 decimal places and truncate rounding
var formatter = new DecimalFormatter(0, RoundingMode.Truncate);

// Create message builder with comma delimiter
IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);

// Create file creator for CSV files
IFileCreator<DataRecord> fileCreator = new CsvFileCreator(messageBuilder);

// Create processor service
var processor = new DataProcessorService(messageBuilder, fileCreator);

// Process some data records
var records = new List<DataRecord>
{
    new() { Id = 1, Name = "Item1", Value = 10.9m }, // Will be formatted to 10
    new() { Id = 2, Name = "Item2", Value = 33.999m } // Will be formatted to 33
};

// Write to CSV file
processor.WriteDataFile(records, "output.csv");

// Get TCP payload for a record
byte[] payload = processor.GetTcpPayload(records[0]);
### Custom Formatting
// 2 decimal places with round up
var formatter = new DecimalFormatter(2, RoundingMode.RoundUp);
IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);

// Value 10.991 will be formatted as "11.00"
### Flexible Message Format System

The new flexible message format system allows you to define client-specific formats for your data:
// Step 1: Create a message format provider to store client-specific formats
var formatProvider = new MessageFormatProvider();

// Step 2: Define formats for different clients
// Example for "Client A" with pipe delimiter
var clientAFormat = new MessageFormat(new[]
{
    FormatToken.CreateLiteral("CONTAINERSTATUS"),  // constant literal value
    FormatToken.CreateKey("ContainerId"),          // value from data dictionary
    FormatToken.CreateKey("ScanAction"),
    FormatToken.CreateKey("Format"),
    FormatToken.CreateKey("Length"),
    FormatToken.CreateKey("Width"),
    FormatToken.CreateKey("Height"),
    FormatToken.CreateKey("Volume"),
    FormatToken.CreateKey("Weight")
}, "|");

// Another client with comma delimiter and different field order
var clientBFormat = new MessageFormat(new[]
{
    FormatToken.CreateLiteral("MYTAG"),
    FormatToken.CreateKey("ScanAction"),
    FormatToken.CreateKey("ContainerId"),
    FormatToken.CreateKey("Length"),
    FormatToken.CreateKey("Width"),
    FormatToken.CreateKey("Height"),
    FormatToken.CreateKey("Weight")
}, ",");

// Step 3: Register the formats with the provider
formatProvider.RegisterFormat("ClientA", clientAFormat);
formatProvider.RegisterFormat("ClientB", clientBFormat);

// Step 4: Create a processor service
var processor = new FlexibleProcessorService(formatProvider);

// Step 5: Create and populate a data record
var record = new FlexibleRecord();
record.SetValue("ContainerId", "317164239");
record.SetValue("ScanAction", "SCANNED");
record.SetValue("Format", "DIMS");
record.SetValue("Length", 44f);
record.SetValue("Width", 35f);
record.SetValue("Height", 38f);
record.SetValue("Volume", 57910f);
record.SetValue("Weight", 13f);

// Step 6: Generate messages for different clients using the same data
string clientAMessage = processor.GetMessage("ClientA", record);
// Result: "CONTAINERSTATUS|317164239|SCANNED|DIMS|44|35|38|57910|13"

string clientBMessage = processor.GetMessage("ClientB", record);
// Result: "MYTAG,SCANNED,317164239,44,35,38,13"

// Step 7: Generate TCP payload for network transmission
byte[] payload = processor.GetTcpPayload("ClientA", record);
### Helper Methods for Common Use Cases
// Create a processor service
var processor = new FlexibleProcessorService(formatProvider);

// Use the helper method to create a pre-configured container status record
var containerRecord = processor.CreateContainerStatus(
    containerId: "317164239",
    scanAction: "SCANNED",
    format: "DIMS",
    length: 44f,
    width: 35f,
    height: 38f,
    weight: 13f
);

// Volume is automatically calculated: length × width × height
## Architecture

The library follows SOLID principles and uses a modular architecture:

- **Abstractions**: Interfaces defining core contracts (IMessageBuilder, IFileCreator)
- **Formatting**: Value formatting implementations (DecimalFormatter, FormatToken, MessageFormat)
- **Implementations**: Concrete implementations of interfaces (FlexibleMessageBuilder, CsvFileCreator)
- **Data Models**: Data containers (DataRecord, FlexibleRecord, ContainerStatusRecord)
- **Services**: Processing services (DataProcessorService, FlexibleProcessorService)
- **Providers**: Registry services (MessageFormatProvider)

### Flexible Message Format Components

The flexible message format system consists of the following components:

- **FormatToken**: Represents a token in a message format (literal value or key reference)
- **MessageFormat**: Defines a format using a collection of tokens and a delimiter
- **FlexibleRecord**: Dictionary-based data container that can store any type of value
- **FlexibleMessageBuilder**: Builds messages from FlexibleRecord objects using a MessageFormat
- **MessageFormatProvider**: Registry of client-specific message formats
- **FlexibleProcessorService**: Service that builds client-specific messages and payloads

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.