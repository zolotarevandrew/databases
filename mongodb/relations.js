//one to one

db.createCollection('one_to_one');

oneToOne = db.getCollection('one_to_one');
oneToOne.insertOne({
	_id: 'joe',
	name: 'Joe One',
	address: {
		street: 'Street',
		city: 'City',
		home: 'Home',
	}
});

db.getCollection('one_to_one').find({ "address.street": "Street"});

//one to many

db.createCollection('one_to_many');

oneToMany = db.getCollection('one_to_many');
oneToMany.insertOne({
	_id: 'joe',
	name: 'Joe One',
	addresses: [{
		street: 'Street',
		city: 'City',
		home: 'Home',
	}]
});

db.getCollection('one_to_many').find({ "addresses.street": "Street"});


db.createCollection('one_to_many_ref1');
db.createCollection('one_to_many_ref2');

oneToManyRef1 = db.getCollection('one_to_many_ref1');
oneToManyRef1.insertOne({
	_id: 'joe',
	name: 'Joe One',
	addresses: [
		{id: '1'},
		{id: '2'}
	]
});

oneToManyRef2 = db.getCollection('one_to_many_ref2');
oneToManyRef2.insertOne({
	_id: '1',
	street: 'Street1',
	city: 'City1',
	home: 'Home1',
});
oneToManyRef2.insertOne({
	_id: '2',
	street: 'Street2',
	city: 'City2',
	home: 'Home2',
});

db.getCollection('one_to_many_ref1').aggregate({
    $lookup: {
      'from': 'one_to_many_ref2', 
      'localField': 'addresses.id', 
      'foreignField': '_id', 
      'as': 'addresses'
    }
});




