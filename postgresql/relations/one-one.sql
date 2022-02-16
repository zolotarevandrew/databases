CREATE TABLE users (
  id serial primary key,
  username text NOT NULL
);

CREATE TABLE addresses (
  user_id serial PRIMARY KEY,
  street VARCHAR(30) NOT NULL,
  CONSTRAINT fk_user_id FOREIGN KEY (user_id) REFERENCES users (id)
);

insert into users(username) values('andrew');
insert into users(username) values('lisa');

insert into addresses(user_id, street) values(1, 'st1');
insert into addresses(user_id, street) values(22, 'st1');