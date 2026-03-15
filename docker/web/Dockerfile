FROM php:8.4-apache

RUN apt-get update && apt-get install -y \
    libpng-dev libjpeg-dev libfreetype6-dev libzip-dev \
    libxml2-dev libicu-dev unzip && rm -rf /var/lib/apt/lists/*

RUN docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install -j$(nproc) mysqli pdo_mysql zip gd bcmath intl soap

ENV APACHE_DOCUMENT_ROOT /var/www
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf
RUN sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

RUN a2enmod rewrite
COPY src/web /var/www/

RUN find /var/www -type f -name "*.php" -exec sed -i 's/\r$//' {} + && \
    chown -R www-data:www-data /var/www/

WORKDIR /var/www/

CMD sed -i "s/define('DB_ADDR', '.*');/define('DB_ADDR', 'hlx-db');/g" /var/www/config.php && \
    sed -i "s/define('DB_USER', '.*');/define('DB_USER', '$DB_USER');/g" /var/www/config.php && \
    sed -i "s/define('DB_PASS', '.*');/define('DB_PASS', '$DB_PASS');/g" /var/www/config.php && \
    sed -i "s/define('DB_NAME', '.*');/define('DB_NAME', '$DB_NAME');/g" /var/www/config.php && \
    apache2-foreground