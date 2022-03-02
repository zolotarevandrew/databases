//tran1
session = db.getMongo().startSession({ readConcern: 'local' });
testCol = session.getDatabase("test").test;
session.startTransaction();

testCol.insertOne({ name: 'Andrew'});

session.commitTransaction();
session.endSession();


//tran2
session = db.getMongo().startSession({ readConcern: 'local' });
testCol = session.getDatabase("test").test;
session.startTransaction();

testCol.findOne({ name: 'Andrew'});


session.commitTransaction();
session.endSession();