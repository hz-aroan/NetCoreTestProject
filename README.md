# NetCoreTestProject - how to install

* create a MS-SQL database somewhere in your server
* select the database (eg. `USE HexaIOTest`)
* execute the `MAIN-01-001-001-001-initialize.sql` script (from `Deployment\SQL-Migration` folder) to create the tables and indexes
* in the `Web.Site\appsettings.json` file update the connection string
* run the Web.Site app (go to section *First look*)
* also in the `Web.API\appsettings.json` file update the connection string to the same
* run the Web.API app, and try the operation throgh the exposed API on the Swagger UI




# First look on WEB

* On the upper right corner there is an *Backoffice* menu where one can add new events and new products
* note: when exception is thrown for some validation reason the default web exception page will displayd (it is not customized yet)
* add some events and products
* then go back to *Home* site (left upper menu)
* select an event to attend (will create a basket)
* the event service fee is displayed in the payment section
* add some products to the basket (+ and - buttons)
* the payment updates
* if one remove all the products from the basket - the service fee payment comes again




# Project Structure

Infrastructure.SQL  where the Entity Framework DB context and Entity definitions are stored

Lib.Domain          where the CQRS commands, queries, and their handlers are coded
Lib.Domain\Features     the commands and queries
Lib.Domain\Services     the DTO classes and service implementations

Web.Site            an ASP.NET Core MVC app to do the job
Web.Site\appsettings.json          edit it when needed (connection string)
Web.Site\Areas\Backoffice          the backoffice operations
Web.Site\Areas\Site                the customers (user) operations

Web.API           a REST API for the system
Web.API\Controllers\V1\Events     some operations for 'Events' and 'Baskets'
Web.API\Controllers\V1\Products   some operations for 'Products'


Test              Unit Tests project
Test\appsettings.json          edit it when needed (connection string)
Test\TestDB\HexaIOTest.mdf     the local .mdf file to do the test
Test\Domaintest                some tests against the domain services directly
Test\ServiceTest               some tests using the dispatcher commands and queries (using the .mdf)






# NetCoreTestProject - the problem

Given a small online store where a user can buy products for events. An event can have many
products. Given an event, the user can buy any number of products in the store. When buying a
product, a service fee is to be added to the total. Service fees can be configured on both event
and product level. When no override is present for a product, the event fee applies.

Constraints:
- Events and products should at least have a "name"; property.
- A service fee consists of a currency and an amount
- For simplicity, you can assume all products cost €10.

Tasks:
Design a small system that expresses these events, products and service fees in a database.
Please ensure all the required files or resources with any setup instructions outside of the main
project folder are sent to us as well.

Then, write a C# application that calculates the total service fee based on an event and a
selection (basket) of products and quantities. A basic page with a form or an API allowing the
requirements to be fulfilled is sufficient but (if you choose the former latter) 
a simple client-side UI updating the database would be preferred.

Write a small unit test suite using any of the C# unit test frameworks demonstrating the
functionality of the program and possible corner cases, full coverage is not needed.

