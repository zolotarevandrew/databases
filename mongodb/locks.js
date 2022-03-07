//tran1
session = db.getMongo().startSession({ readConcern: 'local' });
col = session.getDatabase("test").indexes_test;
session.startTransaction();

col.findOneAndUpdate(
   { "_id" : "621f99f41e9102739945f4d4" },
   { $set: { "address" : "test1"} }
);
sleep(10000);

session.commitTransaction();
session.endSession();

//tran2
session = db.getMongo().startSession({ readConcern: 'local' });
col = session.getDatabase("test").indexes_test;
session.startTransaction();

col.findOneAndUpdate(
   { "_id" : "621f99f41e9102739945f4d4" },
   { $set: { "address" : "test2"} }
);

session.commitTransaction();
session.endSession();

//plan executor error during findAndModify :: caused by :: WriteConflict error