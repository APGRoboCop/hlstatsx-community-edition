# HLstatsX : Community Edition

![PHP Version](https://img.shields.io/badge/PHP-8.4-777bb4.svg?style=flat-square&logo=php)
![Docker Build](https://github.com/lovasatt/hlstatsx-community-edition/actions/workflows/ci.yml/badge.svg)
[![github-release](https://img.shields.io/github/v/release/lovasatt/hlstatsx-community-edition?style=flat-square)](https://github.com/lovasatt/hlstatsx-community-edition/releases/)
![Game](https://img.shields.io/badge/Games-CS2%20Support-orange.svg?style=flat-square)
![GitHub repo size](https://img.shields.io/github/repo-size/lovasatt/hlstatsx-community-edition?style=flat-square&label=Web%20Size&color=blue)

HLstatsX Community Edition is an open-source project licensed
under GNU General Public License v2 and is a real-time stats
and ranking for Source engine based games. HLstatsX Community
Edition uses a Perl daemon to parse the log streamed from the
game server. The data is stored in a MySQL Database and has
a PHP frontend.

Counter-Strike 2 is supported, via [`source-udp-forwarder`](https://github.com/startersclan/source-udp-forwarder).

## :loudspeaker: Important changes

| Date | Description / Feature | Support Status / Additional Information |
| :--- | :--- | :--- |
| **2026-03-14** | **Modern Docker Stack** | **PHP 8.4, Debian 13, Automated DB init & CI verification. 🚀** |
| 2026-02-15 | Modern In-Game Interface | Integrates HLStatsX with Counter-Strike 2 using rich HTML dashboards for rankings, weapon stats, and player management. The in-game menus can be accessed with !mm (EloRank) and !hlx (HLStatsX). |
| 2026-01-28 | Unified Weapon System | Full-Stack Hitgroup & Loadout Refactor for Source 2 🎯 |
| | Generic Hitgroup | Implemented 'Body' (Generic 0) support across C#, Perl, and SQL |
| | Database Update | Update #94: Automated SQL schema migration for 8th hitgroup |
| 2026-01-12 | Core Refactoring | Modular CSS Platform: Adaptive Modes (Normal/Dark) and Mobile UX 📱 |
| | Visual Design | High-contrast Light & Dark modes with brightness-corrected assets |
| 2026-01-02 | Modernized Build | PHP 8.4 and Counter-Strike 2 Support 🚀 |
| | PHP Version | Full PHP 8.4 compatibility (Zero deprecated warnings) |
| | Security | Silent Migration (Legacy MD5 auto-upgrade to `password_hash`) |
| | CS2 Support | Updated Daemon (Correct CT/T fire/inferno differentiation) |
| | Calculations | EloRank System & SuperLogs plugin integration |
| | Architecture | Modernized `/src` directory structure |
| 07.01.2020 | #45 GeoIP2 Update | Linux script updated, GeoLite2 MaxMind database (GDPR) [Ref](https://blog.maxmind.com/2019/12/18/significant-changes-to-accessing-and-using-geolite2-databases/) |

---
### Standalone Installation Guide

All required files are located within the **`/src`** directory.

### 1. Database Setup
1. Create a MySQL/MariaDB database.
2. Import the initial schema: `./src/sql/install.sql`
3. *Note: For upgrades, the system handles the silent password migration automatically.*

### 2. Web Frontend (PHP 8.4)
1. **Upload:** Transfer `/src/web/` contents to your web server's directory.
2. **Document Root:** Point your webserver's Document Root specifically to the `/web` folder.
3. **Configuration:** Edit database credentials in: `./src/web/config.php`
4. **Linux Permissions:** Ensure the web server user (e.g., `www-data`) has proper ownership:
   ```bash
   # Example for Ubuntu/Debian:
   chown -R www-data:www-data /path/to/src/web
   chmod -R 755 /path/to/src/web
   ```
5. **Log Streaming:** To ensure the game server streams logs correctly to your daemon, add the following parameters to your CS2 server startup script (launch options):
   ```bash
   +log on +logaddress_add_http "http://your_ip:26999" (to source-udp-forwarder)
   ```
6. **CS2 Dedicated Server Integration:**
To enable real-time tracking for Counter-Strike 2:<br>
    Deploy Plugins: Copy the pre-compiled plugins from `./src/counterstrikesharp/plugins/` to your server's directory:<br>
    `game/csgo/addons/counterstrikesharp/plugins/`<br>
    Configuration: Update the .json configuration files for plugins with your database credentials.<br>
    Warmup Control: The included Warmup plugin automatically disables logging during warmup periods to prevent erroneous data collection and ensure statistical integrity.<br>
    
Detailed installation instructions below on the wikipedia page
---

## :book: Documentation

- https://github.com/NomisCZ/hlstatsx-community-edition/wiki 🚧 Wiki - work in progress 🚧

## :speech_balloon: Help

- https://forums.alliedmods.net/forumdisplay.php?f=156

---

## Usage (docker)

The modernized way to deploy HLStatsX:CE. Featuring automated setup, this version runs a **Debian 13 (Trixie)** and **PHP 8.4** stack inside the container, ensuring seamless compatibility across **Linux, Windows, and macOS**.

```bash
# 0. Clone the repository (if not done yet) (linux example)
git clone https://github.com/lovasatt/hlstatsx-community-edition.git hlstatsx
cd hlstatsx

# 1. Prepare your environment file
cp .env.example .env

# 2. Configure your settings. Open the .env file (using nano .env or any text editor) and update these three critical values:
DB_PASS: Set your secret database password.
PROXY_KEY: Set your daemon's secret key (must match your game server config).
GAME_SERVER_IP: The IP address of your game server.

# 3. Build and launch the stack
docker compose up -d

# 4. Access the Web Interface. Once the containers are running, open your browser:
URL: http://your-server-ip/ (or http://localhost/)
Default Admin: admin
Default Password: 123456
```
Ensure no other service is using the same ports (default: web 80, daemon 27500, forwarder 26999).

### Docker Compose

The stack is managed via a single `docker-compose.yml` file. Database initialization and configuration are fully automated.

| Service | Internal Port | External Port | Protocol | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Web** | 80 | **80** | TCP | Web Interface (Admin: `admin` / `123456`) |
| **Forwarder**| 26999 | **26999** | TCP/UDP | Connect your cs2 gameserver here (`+logaddress_add_http "http://your-server-ip:26999"`) |
| **Daemon** | 27500 | **27500** | UDP | Internal log processor |

- **Automated Init:** The `install.sql` is automatically imported into the MariaDB container on the first run. 
- **Dynamic Setup:** The `PROXY_KEY` from your `.env` file is automatically injected into the database and configuration files during startup.
- **Port Conflict:** If port 80 is already in use on your host, change the mapping in your `.env` (e.g., `WEB_PORT=81`).

Note: Forwarder is required only for Counter-Strike 2. For other Source engine games, direct daemon logging is sufficient.
When adding your game server via the web interface, set the Daemon IP/Hostname to hlx-daemon (not localhost) so that HLX:CE can properly reload/restart. After changes, reload or restart the daemon for them to take effect.

### Upgrading (docker)

1. **Update code:** Pull the latest changes from the repository.
   ```bash
   git pull origin master
   ```
2. **Rebuild containers:** Restart the stack and rebuild images to apply changes.
   ```bash
   docker compose up -d --build
   ```
3. **Database Migration:** Login to the **Web Admin Panel.** If a schema update is required, a notice will appear. Click the HLX:CE Database Updater button to finish the upgrade.

## Development

Modern deployment uses the **Docker Compose V2** plugin. Use `docker compose` instead of the legacy `docker-compose`.

### 📋 Useful Development Commands

**Accessing containers (Shell):**
```bash
# Web container shell
docker compose exec web bash

# Daemon container shell
docker compose exec daemon bash

# Database container shell
docker compose exec db bash
```
**Manual script execution (inside the daemon):**
```bash
# Generate daily awards
docker compose exec daemon su hlstats -c "perl /home/hlstats/scripts/hlstats-awards.pl"

# Resolve player countries via GeoIP
docker compose exec daemon su hlstats -c "perl /home/hlstats/scripts/hlstats-resolve.pl"
```
**Database Backup & Restore:**

Credentials are read from the .env file automatically if it exists; if not, default values are used.
```bash
# Create a backup (dump)
docker compose exec db mariadb-dump -u ${DB_USER:-hlstatsx} -p${DB_PASS:-hlstatsx_password} ${DB_NAME:-hlstatsx} > backup.sql

# Restore a backup
docker compose exec -i db mariadb -u ${DB_USER:-hlstatsx} -p${DB_PASS:-hlstatsx_password} ${DB_NAME:-hlstatsx} < backup.sql
```
**Live Troubleshooting (Logs):**
```bash
# Follow all logs
docker compose logs -f

# Web interface logs
docker compose logs -f web

# Daemon (Perl log parser) logs
docker compose logs -f daemon

# UDP Forwarder logs from game servers
docker compose logs -f forwarder

# MariaDB container logs
docker compose logs -f db
```
**System Management:**
```bash
# Restart services individually
docker compose restart daemon web forwarder db

# Stop and remove containers (keeps volumes/data)
docker compose down

# FULL RESET: Remove everything (containers, networks, and ALL database data)
docker compose down -v --remove-orphans

# Clean up unused Docker resources
docker system prune -a -f
docker volume prune -f
docker network prune -f
```

## Troubleshooting & FAQ

### Q: I see "Waiting for DB..." in the daemon logs for a long time.
A: This is normal during the first launch. The MariaDB container needs time to initialize the database and import `install.sql`. The daemon will automatically start as soon as the database is ready.

### Q: My game server logs are not appearing in the stats.
A: Check the following:
1. **Proxy key:** Ensure the `PROXY_KEY` in your `.env` matches the key in **Web Admin Panel > HLStatsX Settings**.
2. **Forwarder (CS2 only):** Counter-Strike 2 requires the Forwarder UDP port (`26999`) to be reachable. Other Source engine games can log directly to the daemon.
3. **Daemon IP/Hostname:** When adding a game server in the web interface, set **Daemon IP/Hostname** to `hlx-daemon` (not `localhost`) to ensure the daemon reloads properly.
4. **Forwarder logs (CS2):**
```bash
docker compose logs -f forwarder
```
### Q: How do I change the web port from 80 to something else?
A: Set the `WEB_PORT` variable in your `.env` file (e.g., `WEB_PORT=81`) and then restart the stack:
```bash
docker compose up -d
```