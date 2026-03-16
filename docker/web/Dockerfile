FROM php:8.4-apache

RUN apt-get update && apt-get install -y \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libzip-dev \
    libxml2-dev \
    libicu-dev \
    libcurl4-openssl-dev \
    libonig-dev \
    unzip \
    git \
    && rm -rf /var/lib/apt/lists/*

RUN docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install -j$(nproc) \
        mysqli \
        pdo \
        pdo_mysql \
        zip \
        gd \
        bcmath \
        intl \
        soap \
        sockets \
        mbstring \
        curl \
        xml

ENV APACHE_DOCUMENT_ROOT /var/www
RUN sed -ri -e 's!/var/www/html!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/sites-available/*.conf \
 && sed -ri -e 's!/var/www/!${APACHE_DOCUMENT_ROOT}!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

RUN a2enmod rewrite headers

COPY src/web /var/www/
RUN find /var/www -type f -name "*.php" -exec sed -i 's/\r$//' {} + && \
    chown -R www-data:www-data /var/www/ \
    && chmod -R 755 /var/www

WORKDIR /var/www/

CMD sed -i "s/defineVar(\"DB_ADDR\", '[^']*');/defineVar(\"DB_ADDR\", 'hlx-db');/" /var/www/config.php && \
    sed -i "s/defineVar(\"DB_USER\", '[^']*');/defineVar(\"DB_USER\", '$DB_USER');/" /var/www/config.php && \
    sed -i "s/defineVar(\"DB_PASS\", '[^']*');/defineVar(\"DB_PASS\", '$DB_PASS');/" /var/www/config.php && \
    sed -i "s/defineVar(\"DB_NAME\", '[^']*');/defineVar(\"DB_NAME\", '$DB_NAME');/" /var/www/config.php && \
    apache2-foreground
