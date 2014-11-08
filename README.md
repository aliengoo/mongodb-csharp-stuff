# mongodb-csharp-stuff

## Introduction

Handy MongoDB stuff.

## Web API

When working with MongoDB you'll almost certainly want to create models and handle instances carefully.  But sometimes, especially for those quick and dirty implementations for line-of-business apps, you just can't be bothered to rustle up a class, and build all that repository, validation type stuff.

### Using MCS with OWIN

Include a reference to the _mongodb-csharp-stuff.dll_, and then add the following to your OWIN start-up method.

	app.UseMcs(configuration, "test");

Where `app` is an instance of [`IAppBuilder`](http://msdn.microsoft.com/en-us/library/microsoft.owin.builder.appbuilderextensions_methods(v=vs.113).aspx).

The second argument, or, if being pedantic, given it's an extension method, the third argument, is the name of the ```<connectionString>``` in your applications _web.config_ file.

The connection string name is optional.  If present, a [`MongoDatabase`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/MongoDatabase.cs) instance is created add added to the ```IAppBuilder.Properties``` dictionary with the key _"mongodb.{databaseConnectionStringName}"_, so, in this case _"mongodb.test"_.


### Controllers

[`MongoCrudController`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Controllers/MongoCrudController.cs) and [`MongoQueryController`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Controllers/MongoQueryController.cs) are both [`ObjectId`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Bson/ObjectModel/ObjectId.cs) and `BsonDocument` friendly.

Both types are abstract.  This forces the implementer to create collection specific instances, in the hope they'll do something sensible, like adding an ```Authorize``` attribute.

### [`MongoCrudController`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Controllers/MongoCrudController.cs)

Supports BSON document specific operations within a collection.  It requires an instance of [`IMongoRepository`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Repositories/IMongoRepository.cs).  The second argument to the base constructor is the name of the MongoDB collection.

#### Example

	public class NasdaqController : MongoCrudController
    {
        public NasdaqController(IMongoRepository mongoRepository)
            : base(mongoRepository, "nasdaq")
        {
        }
    }

#### GET {controller}/{id}

The `{id}` must be parseable to an [`ObjectId`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Bson/ObjectModel/ObjectId.cs).  See [`ObjectIdModelBinder`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/ModelBinders/ObjectIdModelBinder.cs).

#### POST {controller}

Posted JSON data will be added to the controllers underlying collection.

[`ObjectId`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Bson/ObjectModel/ObjectId.cs) values must be in a specific format.

	{
		$oid : "545e004128d4e27add806520"
	} 

`ISODate` values must also be in a specific format.  That'll be epoch time.

	{
		$date : 1415464376
	}

#### Example

	{
	    "_id": {
	        "$oid": "545e004128d4e27add806520"
	    },
	    "Date": "2014-11-07",
	    "Open": 4636.89,
	    "High": 4638.8,
	    "Low": 4606.81,
	    "Close": 4632.53,
	    "Volume": 1978830000,
	    "AdjClose": 5000.53,
	    "TestDate": {
	        "$date": 73219197
	    }
	}

### [`MongoQueryController`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Controllers/MongoQueryController.cs)

Supports complex queries and pagination within a collection.  It requires an instance of [`IMongoRepository`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Repositories/IMongoRepository.cs).  The second argument to the base constructor is the name of the MongoDB collection.

#### Example

	public class NasdaqQueryController : MongoQueryController
    {
        public NasdaqQueryController(IMongoRepository mongoRepository)
            : base(mongoRepository, "nasdaq")
        {
        }
    }

#### [`RemoteMongoQuery`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Models/RemoteMongoQuery.cs)

The container for sending a query request to an implementation of [`MongoQueryController`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Controllers/MongoQueryController.cs).

#### Example HTTP *POST* request

	{
	  query : {
	    "Date" : {
	      "$gt" : "2014-01-01",
	      "$lt" : "2014-06-30"
	    }
	  },
	  sortBy : {
	    "Volume" : 1
	  },
	  fields : {
	    "Date" : 1,
	    "Open" : 1,
	    "Close" : 1
	  },
	  page : 1,
	  pageSize: 5
	}

Sending an empty object will only return the first 10 documents.  Sending nothing will return a _500_, so don't do that.

The `query`, `sortBy` and `fields` properties are each converted to [`QueryDocument`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/Wrappers/QueryDocument.cs), [`SortByDocument`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/Wrappers/SortByDocument.cs) and [`FieldsDocument`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/Wrappers/FieldsDocument.cs), respectively, by the [`RemoteMongoQueryConverter`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Converters/RemoteMongoQueryConverter.cs).

Pagination relies on [`PaginationHelper`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Helpers/PaginationHelper.cs) to work out the skip value to pass to the query.  Implementers should also note that two trips to the database are requires due to the page calculation requiring a count.

For AngularJS developers, be careful when sending data with properties prefixed with _$_.  AngularJS will strip these properties out.  To get around this feature, using `JSON.stringify`.

### Repositories

#### [`MongoRepository`](https://github.com/aliengoo/mongodb-csharp-stuff/blob/master/mongodb-csharp-stuff/Repositories/MongoRepository.cs)

Simple wrapper around a [`MongoCollection`](https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/MongoCollection.cs), nought sexy here, although is has a collection indexer.