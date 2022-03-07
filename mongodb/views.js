db.createCollection(
  "test_view",
  {
    "viewOn" : "one_to_many_ref1",
    "pipeline" : [
		{
			$lookup: {
			  'from': 'one_to_many_ref2', 
			  'localField': 'addresses.id', 
			  'foreignField': '_id', 
			  'as': 'addresses'
			}
		}
	]
  }
);

db.getCollection("one_to_many_ref1").aggregate( [
      { $lookup: {
			  'from': 'one_to_many_ref2', 
			  'localField': 'addresses.id', 
			  'foreignField': '_id', 
			  'as': 'addresses'
			
		}
	  },
      { $merge: { into: "test_materialized", whenMatched: "replace" } }
] );

