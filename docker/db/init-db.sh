#!/bin/bash
sed -i 's/\r$//' /docker-entrypoint-initdb.d/init-db.sh
sed -i 's/\r$//' /tmp/install.sql
set -e
mariadb -u root -p"$MYSQL_ROOT_PASSWORD" "$MYSQL_DATABASE" < /tmp/install.sql
mariadb -u root -p"$MYSQL_ROOT_PASSWORD" "$MYSQL_DATABASE" <<-EOSQL
    UPDATE hlstats_Options SET value = '$HLX_PROXY_KEY' WHERE keyname = 'proxy_key';
EOSQL