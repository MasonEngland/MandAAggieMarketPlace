## MANDA marketplace 

this is a fake ecommerce type set meant to help learn about website architecture as well as practice full stack web development


### How to run 

once cloned you need to enter the server directory and create an appsettings.json with a db connection string, my database is not public so will you have to use your own. you will also need to create a .env file with a variable named ACCESS_TOKEN_SECRET this is how we can make secure jwts. finally you can use ` $ dotnet run ` to start the server. by default the server runs on port 2501 but this can be changed in the appsettings.json you create. 


### technologies

the main technologies used are:
* react
* asp.net 
* jwts
* dotenv
* javascript
* html
* css
* css modules
* mySQL

### more details

this app does not perform real transactions but does use real database queries with hashed passwords. the idea is that in order to turn this FAKE ecommerce site into a real ecommerce site would only take some form of transaction function and a form of deployment.
