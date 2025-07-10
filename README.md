# DataProcessor

A .NET library for processing and formatting data records with customizable formatting options.

## Overview

DataProcessor is a flexible .NET 9.0 library that provides components for processing data records, formatting decimal values with different rounding options, and outputting data to CSV files or TCP payloads.

## Features

- Support for different rounding modes (Truncate, RoundUp, RoundDown)
- Configurable decimal precision (0-3 decimal places)
- CSV file creation with customizable formatting
- TCP payload generation for network transmission
- Extensible architecture using interfaces and dependency injection

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later

### Installation

Clone the repository and build the solution:

```bash
git clone https://github.com/yourusername/DataProcessor.git
cd DataProcessor
dotnet build
```

### Running Tests

```bash
dotnet test
```

## Usage Examples

### Basic Usage

```csharp
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
```

### Custom Formatting

```csharp
// 2 decimal places with round up
var formatter = new DecimalFormatter(2, RoundingMode.RoundUp);
IMessageBuilder<DataRecord> messageBuilder = new FixedDelimiterMessageBuilder(",", formatter);

// Value 10.991 will be formatted as "11.00"
```

## Architecture

The library follows SOLID principles and uses a modular architecture:

- **Abstractions**: Interfaces defining core contracts
- **Formatting**: Value formatting implementations
- **Implementations**: Concrete implementations of interfaces

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.