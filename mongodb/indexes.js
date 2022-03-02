db.createCollection('indexes_test');

col = db.getCollection('indexes_test');
col.find({ latitude: 60.234417});

//before was coll scan, after index scan
col.createIndex({ latitude: 1});

col.createIndex({ latitude: 1, longitude: 1});

col.find({ $and: [ {latitude: 58.782711}, {longitude: -0.551937}] });

col.createIndex({ index: 1}, { unique: true} );


col.createIndex(
   { address: 1 },
   { partialFilterExpression: { longitude: { $gt: 150 } } }
)
col.find({ address: "722 Minna Street, Kansas, Arkansas, 5727", longitude: { $gt: 150 }});


db.createCollection('event_logs');

db.getCollection('event_logs').createIndex( { "date": 1 }, { expireAfterSeconds: 20 } )

db.getCollection('event_logs').insertOne({
	date: new ISODate('2022-03-02T00:12:00Z')
});