# apiosf - apisof.net clone
This project is an effort to replicate apisof.net public .net repository.
# Be aware this project is in very early stages and might change a lot.

# Background
The idea behind this project is:
1. Provide a way to document all methods, classes and interfaces signatures
2. Allow correlations between classes, methods (return types, method parameters) and interfaces implementations.
3. Store changes between versions of same assembly (by using assembly versioning)

# Architecture
- AspNet Core to host front end and api to upload nuget packages
- Lucene.Net as package indexer
- Mono.Cecil as disassembler and analyzer.

## Milestones

- Version 0.0
  - ~Be able to store classes into lucene.~
  - ~Be able to store methods into lucene.~
  - Be able to store all properties, events, delegates from classes, structs and interfaces.
  - Be able to render c# code sample
  - Be able to search for classes inside Lucene.
- Version 0.1
  - Be able to search and correlate types loaded into lucene.
  - Store more information about methods.
  - Store more information about classes.
  - Be able to compare different versions of same assembly (either between versions, either between frameworks) with class/method/whatever filtering
- Version 0.2
  - Be able to correlate xml doc stored into nuget package to front end, so the package author can further enhance package documentation.

## What's included into lucene:

Assemblies:
* Assembly name
* Assembly version
* Assembly framework

Classes:
* Class name and namespace
* Class metadata (only public classes), if static or not
* Interfaces implemented
* Class inheritance

Methods:
* Method name
* Return type
* If static or instance

Method parameters:
* Parameter type

All of the information is indexed inside lucene.net
