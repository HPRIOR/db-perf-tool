FROM postgres:13.4
ENV POSTGRES_PASSWORD docker
ENV POSTGRES_USER postgres
ENV POSTGRES_DB postgres
COPY dvdrental.tar /var/lib/postgresql/backup/dvdrental.tar
COPY init-user-db.sh /docker-entrypoint-initdb.d/
CMD ["docker-entrypoint.sh", "postgres"]







