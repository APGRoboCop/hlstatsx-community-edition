FROM debian:trixie-slim

RUN apt-get update && apt-get install -y \
    perl libdbi-perl libdbd-mysql-perl libio-socket-inet6-perl \
    cpanminus build-essential libssl-dev zlib1g-dev \
    libmaxminddb-dev netcat-openbsd cron && rm -rf /var/lib/apt/lists/*

RUN cpanm --notest Math::Int64 MaxMind::DB::Reader GeoIP2::Database::Reader Syntax::Keyword::Try

RUN useradd -m hlstats
WORKDIR /home/hlstats/scripts

COPY src/scripts /home/hlstats/scripts
COPY docker/daemon/hlx-cron /etc/cron.d/hlx-cron

RUN sed -i 's/\r$//' /home/hlstats/scripts/hlstats.conf && \
    sed -i 's/\r$//' /etc/cron.d/hlx-cron && \
    chmod 0644 /etc/cron.d/hlx-cron && \
    chmod +x /home/hlstats/scripts/*.pl && \
    chown -R hlstats:hlstats /home/hlstats/scripts

CMD sed -i "s/DBHost\s*\".*\"/DBHost \"hlx-db\"/" /home/hlstats/scripts/hlstats.conf && \
    sed -i "s/DBUsername\s*\".*\"/DBUsername \"$DB_USER\"/" /home/hlstats/scripts/hlstats.conf && \
    sed -i "s/DBPassword\s*\".*\"/DBPassword \"$DB_PASS\"/" /home/hlstats/scripts/hlstats.conf && \
    sed -i "s/DBName\s*\".*\"/DBName \"$DB_NAME\"/" /home/hlstats/scripts/hlstats.conf && \
    cron && \
    until nc -z hlx-db 3306; do echo "Waiting for DB..."; sleep 2; done; \
    su hlstats -c "cd /home/hlstats/scripts && perl hlstats.pl --port 27500"