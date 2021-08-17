# Arrowhead Client Example
An example Arrowhead client using the [Arrowhead C# client library](https://github.com/97gushan/arrowhead-client-library)

*DISCLAIMER: this demo has only been tested on Linux and although it should run on other platforms no guarantees are given*


## Requirements
* .NET Core Runtime/SDK (tested on 3.0)
* [Arrowhead C# Client library](https://github.com/97gushan/arrowhead-client-library) 
* [Grapevine REST server library](https://github.com/sukona/Grapevine)
* Arrowhead Mandatory Core systems running


### Certificates
The Provider and Consumer systems require a client certificate each for the connection to the Core systems. Documentation on how to create these certificates can be found [here](https://github.com/arrowhead-f/core-java-spring/blob/master/documentation/certificates/create_client_certificate.pdf).

For use of the admin functionality in the Arrowhead C# library a sysop certificate from the local cloud is needed. These admin parts are used for adding Authorization Intracloud rules and Orchestration store entries which can also be done via the [Arrowhead Management Tools](https://github.com/arrowhead-tools/mgmt-tool-js), requests to the API via Swagger or queries directly to the local SQL database.

## Run
To start the Producer and the Consumer move into each folder and run
``` dotnet run```
