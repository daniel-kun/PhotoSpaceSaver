create table file_cache (
	id integer not null unique primary key,
	cloud_id varchar(1024) not null unique,
    filename varchar(1024) not null,
	sha1 char(40) not null,
	size integer(8) not null
);

create index file_cache_idx_sha1 on file_cache(sha1);
