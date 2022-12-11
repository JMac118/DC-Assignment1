# DC-Assignment1 - SOA Calculator
This project was created by Sharron Foo and Joshua Macaulay for assignment one in Distributed Computing at Curtin.
<br />Service-Oriented Architecture
<br />Following the assignment specification within the root directory of the repository, we implemented a web service application that 
mathematical functions can be uploaded to and utilised by users. It requires authentication to make any changes to the web api
where we used a basic token generator to manage user state for logins.
# Internal Sections
Authenticator (.NET WCF/Remoting Server)
<br />Verifies user credentials and generates authentication tokens to manage user state.

Service Provider (ASP.NET Web API)
<br />Provides basic mathematical services to users when utilising REST web requests for those math functions.

Registry (ASP.NET Web API)
<br />Allows authenticated users to upload mathematical functions to the web server in the following JSON format:
<br />{
  <br /> &emsp;“Name”: “ADDTwoNumbers”
  <br /> &emsp;“Description”: “Adding two Numbers” 
  <br /> &emsp;“API endpoint”: “http://localhost:port/ADDTwoNumbers” 
  <br /> &emsp;“number of operands”: 2 
  <br /> &emsp;“operand type”: “integer”
<br />}
<br />Registry functions include the following:
* Publish
* Search
* AllServices
* Unpublish

Service Publishing Console Application (C# Console Application)
<br />A simple console application to publish math services. Interacts with the Registry and Authenticator projects.
<br />Has the following functions:
* Registration
* Login
* Publish Service
* Unpublish Service

Client GUI Application (.NET WPF)
<br />A client application built to invoke and test the services provided by the Service Provider

# Concepts
Integrating multiple projects in a client/server architecture.
<br />Serializing and Deserializing .NET objects to/from JSON using NewtonSoft.
<br />Using RestSharp to perform HTTP requests to web api.
<br />Transferring JSON data between applications through web ports.
<br />Utilising asynchronous tasks to perform multi-threading.
<br />Developing applications within .NET Framework.
