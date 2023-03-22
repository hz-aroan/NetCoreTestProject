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
* / for some reason must double click on the menu to get it work (I'm still thinking what the heck is going on) /
* add some events and products
* then go back to *Home* site (left upper menu)
* select an event to attend (will create a basket)
* the event service fee is displayed in the payment section
* add some products to the basket (+ and - buttons)
* the payment updates
* if one remove all the products from the basket - the service fee payment comes again



# Project Structure

Infrastructure.SQL  where the Entity Framework DB context and Entity definitions are stored

Lib.Domain              where the CQRS commands, queries, and their handlers are coded
Lib.Domain\Features     the commands and queries
Lib.Domain\Services     the DTO classes and service implementations

Web.Site                         an ASP.NET Core MVC app to do the job
Web.Site\appsettings.json        edit it when needed (connection string)
Web.Site\Areas\Backoffice        the backoffice operations
Web.Site\Areas\Site              the customers (user) operations

Web.API							 a REST API for the system
Web.API\Controllers\V1\Events    some operations for 'Events' and creates a basket
Web.API\Controllers\V1\Basket    some operations for 'Baskets'
Web.API\Controllers\V1\Products  some operations for 'Products'


Test						     Unit Tests project
Test\appsettings.json            edit it when needed (connection string)
Test\TestDB\HexaIOTest.mdf       the local .mdf file to do the test
Test\Domaintest                  some tests against the domain services directly
Test\ServiceTest                 some tests using the dispatcher commands and queries (using the .mdf)

Note: the test uses a local .mdf file to execute the DB operations. It attaches its own .mdf file.
So safe to execute the tests.


# Notes

* in the `LIB.Domain\Features\Baskets` commands and queries demonstrates how a domain service can be used 
  behind the walls - all operations redirected to the service itself.
* as the `BasketHandlingService` grows, it can be divided into smaller parts of course
* in the `LIB.Domain\Features\Products` I demonstrate how to handle the commands and queries *in place*, 
  without using a domain service. Only a validator service I wrote as later there can be an `ProductUpdateCmd`
  so the re-use of the same validations rules I'm excepting
* similar to this the `LIB.Domain\Features\Events` I created
* the Currency service is based on an 'in-memory' list of available currencies. It is easy to add a currency
  table to the system and replace the operations using the underlying table instead - if needed


Ahh! I have some words on Basket UID (GUID field)! As I told the guid field is not optimal for indexing and
querying data. I have 2 reasons to use guid now:
* basket head table: might not will contains millions of records
* as there is no user table in the system, no authorization, no validation which user owns the basket,
  and the `int` typed `basketId` field is not a safe choose. When you have a basketId you can easily find out
  other basketIds, and might add new products to another basket. Using GUID is a little bit safer managing
  a basket.

Also, as there is no user table - no authorization - so cannot guess which was my own basket for an event before.
So cannot *continue* to work with my previous basket. A basket is lost on WEB if you choose another event to attend
(another basket is created). On Rest.API if you saved you basketUid - you can anytime continue the work 
with the given basket.


# Notes

I copied some codes from my projects for this application
* the CQRS is developed by me (based on MediatR package) - found in `LIB.Domain\Services\CQ`
* some ajax support added to the Web.Site project to enable Modal dialog loading on-the-fly
  and lazyl loading of products list
* added a poorly themed Bootstrap4
* all of them was totally written by me before, just added to this project in a hurry as I think
  it is not the main point for now



