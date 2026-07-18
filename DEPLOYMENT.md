# Deployment Notes

Hosting/deploy infrastructure is intentionally left undecided for this project (see
`docs/superpowers/specs/2026-07-18-dangphatflex-website-design.md`, "Ngoài phạm vi").
This document covers the one operational step every deploy target needs regardless
of platform: activating the admin account.

## Admin account seeding

The admin account is **not seeded in production by default** — this is intentional
and secure. `IdentitySeeder.SeedAsync` (`src/DangPhatFlex.Web/Data/IdentitySeeder.cs`)
only creates the `Admin` role and the admin user when `AdminSeed:Email` and
`AdminSeed:Password` are present in configuration; otherwise it silently no-ops.
Those keys exist only in `src/DangPhatFlex.Web/appsettings.Development.json`, which
is for local development only.

To activate the admin account on a real deploy, set the following as environment
variables before or after the first deploy (ASP.NET Core's environment-variable
configuration provider uses `__` as the section separator, not `:`):

```
AdminSeed__Email=your-admin@yourdomain.com
AdminSeed__Password=<a strong, unique password>
```

Concrete ways to set these, depending on your hosting platform:

- **Environment variables** on the host/container running the app (e.g. a systemd
  unit's `Environment=`, a Docker `-e`/`environment:` entry, or `dotnet user-secrets`
  for local non-Development testing).
- **Azure App Service**: set them under Configuration -> Application settings
  (these are exposed to the app as environment variables automatically).
- A mounted `appsettings.Production.json` with the `AdminSeed` section, placed on
  the server outside of source control — this repo intentionally does not commit
  an `appsettings.Production.json`.

On the next app restart after setting these, `IdentitySeeder` creates the `Admin`
role (if missing) and the one admin user. The seeder is idempotent
(`RoleExistsAsync` / `FindByEmailAsync` guard against duplicates), so it's safe to
leave the env vars set permanently — you may also remove or rotate them afterward
if you'd rather not have the seeder re-check on every restart.

**Never reuse the development credentials** (`admin@dangphatflex.vn` /
`ChangeMe123!` from `appsettings.Development.json`) in production.
