CREATE TABLE books (
  id serial primary key,
  title VARCHAR(100) NOT NULL
);

CREATE TABLE reviews (
  id serial primary key,
  book_id int NOT NULL,
  content VARCHAR(255),
  FOREIGN KEY (book_id) REFERENCES books(id)
);

insert into books(title) values('book1');
insert into books(title) values('book2');
insert into books(title) values('book3');

insert into reviews(book_id, content) values(1, 'review1 1');
insert into reviews(book_id, content) values(1, 'review2 1');
insert into reviews(book_id, content) values(1, 'review2 1');

insert into reviews(book_id, content) values(2, 'review2 1');
insert into reviews(book_id, content) values(2, 'review2 1');
insert into reviews(book_id, content) values(2, 'review2 1');

insert into reviews(book_id, content) values(3, 'review2 1');
insert into reviews(book_id, content) values(3, 'review2 1');
insert into reviews(book_id, content) values(3, 'review2 1');

select b.id, b.title, count(r.id) from books b
join reviews r on r.book_id = b.id
group by b.id, b.title
order by b.title