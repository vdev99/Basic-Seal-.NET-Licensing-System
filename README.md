# Basic Seal .NET Licensing System

Basic Seal is a simple easy to use licensing system for .NET Core 3.1 applications with Windows and Linux support. 

**Note: The project is not production ready, especially the frontend part**

## Short description

The licensing system consists of 3 parts, a website, a REST server and a client library. 
The website was created with the Blazor server-side framework, its purpose is to provide a management interface for the user where the user can add his software to the licensing system. 
With the help of the website the user can manage the versions of the added software, license keys can be generated. The user can edit, delete or search licenses with various filtering functions. 

The data of the users and the data of the licensed applications are stored on the REST server with the help of the MySQL database. 
Both the website and the client library communicate with the REST server to perform various operations. 
All operations performed by the user on the website take place on the server side safely. 
The client library uses the server to retrieve the software license or to authenticate the license. 
The task of the client library is to protect the software of the user from unauthorized use with license verification and other functions. 
The client library can be used in both Windows and Linux operating systems (.NET Core 3.1 is required).

## Guide

**Requirements:** 
- Visual Studio 2019
- .NET Core 3.1 SDK
- MySql

\
\
**Step 1**

Open MySql command line interface, log in and enter the following commands to create a new user for Basic Seal.

CREATE USER 'basicseal'@'localhost' IDENTIFIED BY 'akademiasovy';\
ALTER USER 'basicseal'@'localhost' IDENTIFIED WITH mysql_native_password BY 'akademiasovy';\
GRANT ALL PRIVILEGES ON * . * TO 'basicseal'@'localhost';\
FLUSH PRIVILEGES;

Feel free to change to username and password, but do not forget to also change it in the **BasicSealBackend\appsettings.json** file in the database connection string.

\
**Step 2**

Run the BasicSealBackend project, it will automatically create the database.\
Run the BasicSealBlazor project.

\
**Step 3**

Open your browser and open the default address of the Basic Seal control panel http://localhost:7000/
Create an account and log-in.\
Create a new application

\
**Step 4**

Open the example project in **BasicSealClient_ExampleUse\mp3converter_EXAMPLE**\
Open the BasicSealClient project, and build it, you will need the built **BasicSealClient.dll** in the next step.

\
**Step 5**

In the **BasicSealClient_ExampleUse\mp3converter_EXAMPLE** project right click on the dependencies on the left side in the solution explorer, select Add COM Reference, and reference the previously built **BasicSealClient.dll** library (only reference it if it is missing, it should be already referenced).

\
**Step 6**

In the **BasicSealClient_ExampleUse\mp3converter_EXAMPLE** project in the Main method (Program.cs) where the **BasicSeal.Start** method is called, the parameters need to be set correctly, the first parameter is the software version, it can be any string, the second is the Software ID (from the Manage Versions page in the created application which was created in step 3), the third is if message boxes are enabled or not (feel free to choose) and the fourth parameter is the developer mode switcher (leave it to true).

\
**Step 7**

Build the **BasicSealClient_ExampleUse\mp3converter_EXAMPLE** project, you will need the built **mp3converter.dll** in the next step.\
Build the **HashChecker project**, you will need the built **HashChecker.exe** in the next step.

\
**Step 8**

Generate the hash of the **mp3converter.dll** by entering its path in the **HashChecker.exe**.\
Drag and drop is also supported, drag and drop the **mp3converter.dll** to the **HashChecker.exe** to generate the hash.\
The generated hash is automatically copied to the clipboard.

\
**Step 9**

Go to the Basic Seal control panel (opened in step 3) to the Manage Versions page and enter the **copied hash**, and the **app version** you entered as parameter for the **BasicSeal.Start** method in the **BasicSealClient_ExampleUse\mp3converter_EXAMPLE** project.\
The **software version** and the **software id** in the code must match with the ones added in the control panel.

\
**Step 10**

Run mp3converter.exe, or run the project from Visual Studio.

\
**Step 11**

Try it out!, generate a license key on the website, and enter it to the application.



## Visual example from the guide


![image](https://user-images.githubusercontent.com/86075693/123267410-44bd4080-d4fd-11eb-8fa3-05f438b53277.png)

\
The client side license key verification uses code from https://github.com/appsoftwareltd/dotnet-licence-key-generator
