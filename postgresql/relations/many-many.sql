CREATE TABLE users_books (
  id serial primary key,
  user_id int NOT NULL,
  book_id int NOT NULL,
  FOREIGN KEY (user_id) REFERENCES users(id),
  FOREIGN KEY (book_id) REFERENCES books(id)
);

insert into users_books(user_id, book_id) values(1,1);
insert into users_books(user_id, book_id) values(1,2);
insert into users_books(user_id, book_id) values(1,3);

insert into users_books(user_id, book_id) values(2,1);
insert into users_books(user_id, book_id) values(2,2);
insert into users_books(user_id, book_id) values(2,3);

select u.username, count(b.id) from users_books ub
join users u on u.id = ub.user_id
join books b on b.id = ub.book_id
group by u.username