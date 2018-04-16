# Documentation

This is a command line utility to send database data to Zabbix Server

### Prerequisite: 
	- Microsoft Windows
	- .NET Framework 4.5
	- ODBC drivers

### Usage:
	- To run with default parameter file parameter.json
	ZAbbixMultiDBA 

	- To run with especific parameter file:
	ZAbbixMultiDBA parameterFile

	- To create a Zabbix xml host configuration file
	ZAbbixMultiDBA parameterFile xml

Example: The file parameter.json is an example of configuration. Just pay attention on Database and IDm you have to put this ID on each Select-Config.

	
Tip: You can have multiple parameter files to run database scripts in different periods 
