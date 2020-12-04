# KnotSlack
Welcome to my Slack Clone!
This is a Chat application created using React.js on the front end, an Asp.net Core back end with Entity Framework. The real time chat is powered by Signalr.
It allows for messages to be sent to everyone, a specific group, or a specific user in real-time.

Any Comments/Advice would be appreciated!

## What is SignalR?
[Introduction to SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.1)
SignalR is a library for adding real time functionality to applications using web sockets. For those who are unfamiliar, a usual HTTP connection is only one way. The client sends a request to the server and the server responds. A web socket allows for two way communication. In SignalR, the client can call hub(on the server) methods, and the hub can call client methods. 

## Using KnotSlack 

1. Clone the repos. React Front end repo: https://github.com/bproorda/KnotSlackReact

2. Open up the SignalrApi directory in Visual Studio, make sure you have all the required packages.

3. You can open the front end in Visual studio code. Make sure you run npm install. Then use npm start to well, start.

4. The API will open to Swagger documentation. Please note that these do not include the Signalr hub methods.

5. The front end will open to the homepage. Register as a user to begin, login, or enter as guest user.
    -NOTE: a guest user will only have access to General Chat

6. Chat Away! You will start off in the General Chat Area.

7. You can click a button on under Your Channels to open an existing channel, group or private.

8. Click Create group to create a new Group chat channel.

9. Select from the drop down menu to create a new private channel.

10. Please inform the developer of any pesky errors!

## More About KnotSlack

- KnotSlack utlizes three kinds of Chat Windows: General, Group, and Private.

  - General Chat: This uses the hub to send a message to all users.
 
  - Group: Sends messages to a named group to which users are assigned.
 
  - Private: Sends messages to a specific user only.
  
  - Each window takes care of sending messages, but they are all received by the Hub Context
 
- This application has two React contexts

  - User: The center for all authentication/authorization data and methods. Sends information to api using HTTP for Login/Logout
  
  - Hub: This is where all the SignalR/Chat magic happens. You can tell from all the props and methods!
    - Establishes connection to hub on server
    - receives new and processes new messages
    - creates chat windows from User info
