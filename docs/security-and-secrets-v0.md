# Security and Secrets Review v0

**Scope:** Local proof of concept and GitHub Actions CI. No AWS or Azure application deployment is configured.

## Secret Handling

- **Local API connection string:** Development credentials are stored outside the repository in ASP.NET Core User Secrets under `ConnectionStrings:TeleportDatabase`. The project contains a `UserSecretsId`, not the secret value.
- **Local Compose:** `POSTGRES_PASSWORD` can be supplied through `.env` or the shell environment. `.env` and `.env.*` are ignored by Git; `.env.example` contains only a placeholder. Compose binds database, API, and web ports to `127.0.0.1` for local use.
- **GitHub Actions:** CI generates a random database password for the ephemeral test stack and exposes it only to later steps through the runner environment. The workflow has `contents: read` permission and removes the test containers and volume afterward. No deployment credential is configured.
- **Azure upload preparation:** `scripts/upload-data-to-azure.sh` uses Azure CLI login (`--auth-mode login`), does not accept or store an account key/SAS token, and checks for an active login before making cloud changes. A working subscription/storage account has not been available, so no Azure upload or role assignment has occurred. The intended minimum data-plane role is `Storage Blob Data Contributor` on the approved storage scope.
- **API keys:** The current application does not call third-party services and has no API-key configuration.
- **AWS:** No AWS resource or credential is configured in the project. The AWS sign-in notice says the free plan ended, the account is closed, and content may be deleted after December 30, 2026. Restoring/upgrading the account may incur charges.

## Access and Exposure

- GitHub Actions requests only repository-content read permission.
- The local API and database routes do not implement user authentication or authorization. This is a prototype limitation; do not expose this build as a public production service.
- CORS is restricted to the local Angular origin `http://localhost:4200`.
- Azure/AWS identities, network rules, and managed secret stores are not configured because the cloud runtime is not deployed.

## Verification and Limitations

- The local `.env` file is absent and ignored; `.env.example` has a placeholder only.
- A high-confidence scan of the current workspace found no AWS access-key, GitHub-token, or private-key patterns. This scan is not a full historical Git-secret audit.
- The container build's `npm ci` reported 42 dependency advisories (27 moderate, 13 high, 2 critical). `npm audit --omit=dev` found 0 vulnerabilities in production dependencies. Review and update the development/build dependency tree before production use; no forced upgrade was applied.
- Before production deployment, add authentication/authorization, managed secret storage (such as Azure Key Vault or an equivalent), restricted cloud networking, and monitoring. Reassess CORS and database access for the deployed topology.