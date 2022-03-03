
//addFields
db.getCollection('indexes_test').aggregate( [
   {
     $addFields: {
       diff: { $add: [ "$latitude", "$longitude" ] }
     }
   }
] );

//bucket
db.getCollection('indexes_test').aggregate( [
   {
     $bucket: {
      groupBy: "$latitude",                        
      boundaries: [ 50, 100, 150 ],
      default: "Other",                             
      output: {                                     
        "count": { $sum: 1 },
        "artists" :
          {
            $push: {
              "address": "$address"
            }
          }
      }
    }
  },
] );

//facet
db.getCollection('indexes_test').aggregate( [
	{
		$match: { index : 0}
	},
   {
     $facet: {
      "categorizedByTags": [
        { $unwind: "$tags" }
      ],
	 }
    }
] );

//group
db.getCollection('indexes_test').aggregate( [
   {
     $group: {
       _id : "$index",
       sum: { $sum: "$latitude" }
	 }
    }
] );


//lookup
db.getCollection('one_to_many_ref1').aggregate({
    $lookup: {
      'from': 'one_to_many_ref2', 
      'localField': 'addresses.id', 
      'foreignField': '_id', 
      'as': 'addresses'
    }
});

//match
db.getCollection('indexes_test').aggregate( [
   {
     $match: {
       index : 0
	 }
    }
] );

//merge like a materialized view
db.getCollection('indexes_test').aggregate( [
   {
     $match: {
       index : 0
	 }
   },
   { $merge: 'merge' } 
] );

//project
db.getCollection('indexes_test').aggregate( [
   {
     $match: {
       index : 0
	 }
   },
   {
	   $project: {
		   address: 1
	   }
   }
] );


//unwind
db.getCollection('indexes_test').aggregate( [ { $unwind : "$tags" } ] )
