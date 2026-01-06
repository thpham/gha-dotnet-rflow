# MyApp

A .NET 9 Windows application reference blueprint with automated release management using GitHub Actions, Release Please, dotnet-releaser, and WiX Toolset v4 for MSI installers.

## Features

- **ASP.NET Core Web API** with Windows Authentication and API key support
- **Automated releases** via Release Please (versioning, changelog)
- **Multi-architecture builds** via dotnet-releaser (win-x64, win-arm64)
- **MSI installer creation** via WiX Toolset v4 with IIS integration
- **Conventional commits** enforcement with commitlint
- **Cherry-pick automation** for backporting fixes to release branches
- **Automated dependency updates** via Dependabot (NuGet + GitHub Actions)
- **Code quality analysis** via SonarCloud
- **GitHub Actions security** best practices (SHA pinning, cache protection)
- **Integration testing** with Pester PowerShell tests
- **Local workflow testing** with nektos/act

## Prerequisites

- .NET 9 SDK (Windows)
- Visual Studio 2022 or VS Code
- PowerShell 5.1+ with Pester v5
- WiX Toolset v4 (for installer builds)
- [pre-commit](https://pre-commit.com/) (for Git hooks)

### Using Nix (Recommended for macOS/Linux)

This project includes a `flake.nix` for reproducible development environments:

```bash
# Enter the development shell (installs all tools automatically)
nix develop

# Or with direnv (automatic activation)
direnv allow
```

The Nix shell provides: .NET 9 SDK, Node.js (commitlint), actionlint, zizmor, act, pre-commit, and more.

## Quick Start

```bash
# Clone the repository
git clone https://github.com/thpham/gha-dotnet-rflow.git
cd gha-dotnet-rflow

# Build and test
dotnet build MyApp.sln
dotnet test

# Run the API locally
cd src/MyApp
dotnet run

# Access the API
curl http://localhost:5000/health
```

## Project Structure

```
.
├── .github/
│   ├── workflows/
│   │   ├── ci.yml              # Build, test, lint, MSI preview
│   │   ├── release.yml         # Release Please + dotnet-releaser + MSI
│   │   ├── backport.yml        # Cherry-pick automation
│   │   ├── cleanup.yml         # Weekly artifact cleanup
│   │   ├── lint-workflows.yml  # actionlint + zizmor security scanning
│   │   ├── commitlint.yml      # Conventional commit validation
│   │   └── sonar.yml           # SonarCloud analysis
│   ├── dependabot.yml          # Automated dependency updates
│   ├── labeler.yml             # PR auto-labeling rules
│   ├── actionlint.yaml         # actionlint configuration
│   ├── PULL_REQUEST_TEMPLATE.md
│   └── CODEOWNERS
├── src/
│   └── MyApp/
│       ├── Controllers/        # API endpoints
│       ├── Services/           # Business logic
│       ├── Options/            # Configuration classes
│       ├── RestApis/           # External API clients
│       ├── Program.cs
│       └── MyApp.csproj
├── tests/
│   ├── MyApp.Tests/            # xUnit tests
│   │   ├── Unit/
│   │   └── Integration/
│   └── Scripts/                # Pester PowerShell tests
├── installer/
│   ├── Package.wxs             # WiX v4 main package
│   ├── Directories.wxs
│   ├── Features.wxs
│   ├── IISConfiguration.wxs
│   └── Assets/
├── Directory.Build.props       # Centralized version management
├── MyApp.sln
├── release-please-config.json
├── .release-please-manifest.json
├── dotnet-releaser.toml
├── commitlint.config.mjs
├── sonar-project.properties
├── .actrc                      # nektos/act configuration
└── CHANGELOG.md
```

## Release Flow

This project follows the **Release Flow** branching strategy:

```
main (development)
  │
  ├── feature/xyz → PR → main (squash merge)
  │                   │
  │                   └── Merge → Release Please PR → Merge → v1.2.3
  │                                                      │
  │                                                      └── dotnet-releaser:
  │                                                          - Multi-arch builds
  │                                                          - GitHub Packages
  │                                                          - MSI installers
  │
  └── release/1.x (LTS/maintenance)
        └── Backport via label: "backport release/1.x"
```

### Creating a Release

1. Merge PRs with conventional commit messages to `main`
2. Release Please automatically creates a Release PR
3. Review and merge the Release PR
4. dotnet-releaser builds and publishes artifacts

### Backporting

This project follows trunk-based development best practices: **fixes are committed to `main` first**, then cherry-picked to release branches.

#### Automatic Label Suggestions

When a PR contains `fix:` or `security:` commits, backport labels are **automatically suggested**:

| PR Target     | Auto-Suggested Labels                                    |
| ------------- | -------------------------------------------------------- |
| `main`        | `backport release/X.Y` for the 2 latest release branches |
| `release/X.Y` | `backport main` + other active release branches          |

**Workflow:**

1. Open a PR with `fix:` or `security:` commits
2. Labels like `backport release/1.2` are automatically added
3. **Review the suggestions** - remove labels for branches that shouldn't receive the fix
4. Merge the PR
5. Cherry-pick PRs are automatically created for remaining labels

#### Manual Backporting

To manually backport any PR:

1. Merge the fix to `main` first
2. Add label `backport release/1.x` to the merged PR
3. A cherry-pick PR is automatically created

> **Note**: If cherry-pick fails due to conflicts, an issue is created for manual resolution.

## Commit Conventions

This project uses [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### Types

| Type       | Description   | Version Bump | Auto-Backport |
| ---------- | ------------- | ------------ | ------------- |
| `feat`     | New feature   | Minor        | No            |
| `fix`      | Bug fix       | Patch        | **Yes**       |
| `security` | Security fix  | Patch        | **Yes**       |
| `docs`     | Documentation | Patch        | No            |
| `perf`     | Performance   | Patch        | No            |
| `refactor` | Refactoring   | Patch        | No            |
| `test`     | Tests         | None         | No            |
| `ci`       | CI/CD         | None         | No            |
| `build`    | Build system  | None         | No            |
| `chore`    | Maintenance   | None         | No            |

### Scopes

- `app` - Main application
- `api` - API endpoints
- `tests` - Test projects
- `installer` - WiX installer
- `deps` - Dependencies
- `ci` - CI/CD workflows
- `release` - Release configuration

### Examples

```bash
feat(api): add user authentication endpoint
fix(app): resolve null reference in health check
security(api): sanitize user input to prevent XSS
docs: update README with Docker instructions
refactor(tests): simplify integration test setup
ci(deps): bump actions/checkout to v4
```

## CI/CD Workflows

| Workflow                | Trigger                    | Purpose                                          |
| ----------------------- | -------------------------- | ------------------------------------------------ |
| `ci.yml`                | PR to main/develop         | Build, test, lint, MSI preview                   |
| `release.yml`           | Push to main/release/\*\*  | Release Please + dotnet-releaser + MSI           |
| `suggest-backports.yml` | PR opened/ready_for_review | Suggest backport labels for fix/security commits |
| `backport.yml`          | Merged PR with label       | Cherry-pick to release branches                  |
| `cleanup.yml`           | Weekly schedule            | Delete old artifacts and workflow runs           |
| `lint-workflows.yml`    | Push/PR changing workflows | actionlint + zizmor security scanning            |
| `commitlint.yml`        | PR to main/develop         | Validate conventional commit messages            |
| `sonar.yml`             | Push/PR with code changes  | SonarQube code quality analysis                  |

## Preview MSI Installers

To build preview MSI installers for your PR:

1. Add the `preview-msi` label to your PR
2. CI will build MSI installers for both x64 and arm64
3. A comment with download instructions will be added to the PR

**Automatic rebuilds**: Once the label is added, every new commit pushed to the PR will automatically build new installers.

**Download options**:

- From the workflow run artifacts section in GitHub UI
- Using GitHub CLI: `gh run download <run-id> -n msi-installer-preview-x64`

> **Note**: This builds installers for manual testing - it does not publish to any release.

## GitHub Actions Security

This project follows security best practices for GitHub Actions workflows, validated by automated tooling.

### Security Measures

| Practice                          | Description                                                                         |
| --------------------------------- | ----------------------------------------------------------------------------------- |
| **Pinned Actions**                | All actions pinned to SHA hashes (not version tags) to prevent supply chain attacks |
| **Minimal Permissions**           | Explicit `permissions:` blocks at job level with least-privilege principle          |
| **Cache Poisoning Prevention**    | Read-only cache restore for PRs; cache writes only on push events                   |
| **Credential Isolation**          | `persist-credentials: false` on all checkout steps                                  |
| **Fork Protection**               | PR workflows verify `head.repo.full_name == github.repository`                      |
| **Template Injection Prevention** | User-controlled data passed via `env:` blocks, not inline `${{ }}`                  |

### Security Scanning Tools

| Tool                                              | Purpose                                   | Install                   |
| ------------------------------------------------- | ----------------------------------------- | ------------------------- |
| [actionlint](https://github.com/rhysd/actionlint) | Workflow syntax and shellcheck validation | `brew install actionlint` |
| [zizmor](https://github.com/woodruffw/zizmor)     | Security vulnerability scanning           | `brew install zizmor`     |

### Running Security Scans

```bash
# Syntax and shell script validation
actionlint

# Security vulnerability scan
zizmor .github/workflows/

# Both tools should report no findings
```

### Suppressed Findings

Some security findings are intentionally suppressed with documented justifications:

| Rule                 | Workflow              | Justification                                                                                                                   |
| -------------------- | --------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| `dangerous-triggers` | backport.yml          | `pull_request_target` required for backport action write permissions. Mitigated by fork check and `persist-credentials: false`. |
| `dangerous-triggers` | suggest-backports.yml | `pull_request_target` required for adding labels/comments. Mitigated by fork check and no PR code execution.                    |

Suppressions use inline comments: `# zizmor: ignore[rule-name]`

### Action Version Reference

All actions are pinned to SHA hashes with version comments for maintainability:

```yaml
- uses: actions/checkout@8e8c483db84b4bee98b60c0593521ed34d9990e8 # v4
```

To update actions, use [Dependabot](https://docs.github.com/en/code-security/dependabot) which is pre-configured in this project.

## Development

### Setting Up Pre-commit Hooks

This project uses [pre-commit](https://pre-commit.com/) to run quality checks before each commit:

```bash
# Install pre-commit (choose one)
brew install pre-commit          # macOS
pip install pre-commit           # Python/pip
pipx install pre-commit          # pipx (isolated)

# Install the Git hooks
pre-commit install
pre-commit install --hook-type commit-msg

# Run all hooks manually (optional)
pre-commit run --all-files
```

**Hooks included:**

| Hook                  | Purpose                                 |
| --------------------- | --------------------------------------- |
| `trailing-whitespace` | Remove trailing whitespace              |
| `end-of-file-fixer`   | Ensure files end with newline           |
| `check-yaml`          | Validate YAML syntax                    |
| `check-json`          | Validate JSON syntax                    |
| `detect-secrets`      | Scan for accidentally committed secrets |
| `actionlint`          | GitHub Actions workflow validation      |
| `dotnet-format`       | C# code formatting                      |
| `commitlint`          | Conventional commit message validation  |

**Skipping hooks** (when needed):

```bash
# Skip all hooks
git commit --no-verify -m "message"

# Skip specific hook
SKIP=dotnet-format git commit -m "message"
```

### Running Tests

```bash
# All tests
dotnet test

# Single project
dotnet test tests/MyApp.Tests

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Pester tests (PowerShell)
Invoke-Pester -Path ./tests/Scripts -Output Detailed
```

### Code Formatting

```bash
# Check formatting
dotnet format --verify-no-changes

# Apply formatting
dotnet format
```

### Local MSI Build

```bash
# Install WiX Toolset
dotnet tool install --global wix

# Add WiX extensions
wix extension add -g WixToolset.UI.wixext WixToolset.Util.wixext WixToolset.Iis.wixext

# Build the application
dotnet publish src/MyApp/MyApp.csproj -c Release -r win-x64 --self-contained -o ./publish

# Build MSI
cd installer
wix build Package.wxs IISConfiguration.wxs Directories.wxs Features.wxs `
  -ext WixToolset.UI.wixext -ext WixToolset.Util.wixext -ext WixToolset.Iis.wixext `
  -arch x64 -d ProductVersion=1.0.0 -d InstallerPlatform=x64 -d PublishDir=../publish `
  -o ../artifacts/MyApp-1.0.0-win-x64.msi
```

## Code Quality (SonarCloud)

This project supports code quality analysis with SonarCloud.

### Setup

1. Create a project at [sonarcloud.io](https://sonarcloud.io)
2. Add `SONAR_TOKEN` secret to your GitHub repository

### Run Locally

```bash
# Install SonarScanner
dotnet tool install --global dotnet-sonarscanner

# Run analysis (requires SONAR_TOKEN environment variable)
dotnet sonarscanner begin /k:"thpham_gha-dotnet-rflow" /o:"thpham" /d:sonar.token=$SONAR_TOKEN
dotnet build
dotnet test --collect:"XPlat Code Coverage"
dotnet sonarscanner end /d:sonar.token=$SONAR_TOKEN
```

## Testing GitHub Actions Locally

This project includes tools for testing GitHub Actions workflows locally before pushing.

### Tools

| Tool                                              | Purpose                         | Install                   |
| ------------------------------------------------- | ------------------------------- | ------------------------- |
| [actionlint](https://github.com/rhysd/actionlint) | Workflow syntax validation      | `brew install actionlint` |
| [zizmor](https://github.com/woodruffw/zizmor)     | Security vulnerability scanning | `brew install zizmor`     |
| [act](https://github.com/nektos/act)              | Run workflows locally           | `brew install act`        |

### Quick Validation (No Docker)

```bash
# Lint all workflows
actionlint

# Security scan
zizmor .github/workflows/

# Lint specific workflow
actionlint .github/workflows/ci.yml
```

### Local Execution with act

```bash
# List available workflows/jobs
act -l

# Run push event (simulates push to main)
act push

# Run specific job
act -j build-and-test

# Dry run (show what would run)
act -n

# Run with secrets
cp .secrets.example .secrets  # Edit with your values
act --secret-file .secrets
```

### Limitations

**act** cannot fully simulate all GitHub Actions features:

| Workflow              | act Support | Notes                              |
| --------------------- | ----------- | ---------------------------------- |
| ci.yml                | Partial     | Windows runner simulated on Linux  |
| sonar.yml             | Partial     | Requires SONAR_TOKEN               |
| release.yml           | Limited     | release-please integration complex |
| suggest-backports.yml | Limited     | Requires PR event with commits     |
| backport.yml          | Limited     | Requires merged PR event           |
| cleanup.yml           | Limited     | Requires GitHub API access         |

**Recommendation**: Use `actionlint` for syntax validation (catches most issues), and test complex workflows via short-lived branches.

## GitHub Repository Settings

Before the CI/CD workflows can function properly, configure the following repository settings.

### Required Permissions

Navigate to **Settings** → **Actions** → **General** → **Workflow permissions**:

| Setting                                                      | Value   | Required For                                       |
| ------------------------------------------------------------ | ------- | -------------------------------------------------- |
| **Allow GitHub Actions to create and approve pull requests** | Enabled | Release Please (creates release PRs automatically) |

> **Why is this needed?** Release Please creates a pull request to track version bumps and changelog updates. Without this permission, the workflow fails with: `GitHub Actions is not permitted to create or approve pull requests`.

### Required Secrets

Navigate to **Settings** → **Secrets and variables** → **Actions**:

| Secret          | Required       | Description                                  |
| --------------- | -------------- | -------------------------------------------- |
| `SONAR_TOKEN`   | For SonarCloud | Token for SonarCloud code analysis           |
| `CODECOV_TOKEN` | For Codecov    | Token for code coverage reporting (optional) |

> **Note**: `GITHUB_TOKEN` is automatically provided by GitHub Actions - no configuration needed.

### Repository Variables (Optional)

Navigate to **Settings** → **Secrets and variables** → **Actions** → **Variables**:

| Variable            | Description                                       |
| ------------------- | ------------------------------------------------- |
| `TEAMS_WEBHOOK_URL` | Microsoft Teams webhook for release notifications |

## API Endpoints

| Endpoint             | Method    | Auth         | Description                 |
| -------------------- | --------- | ------------ | --------------------------- |
| `/api/data`          | GET, POST | Windows Auth | SOAP-style data retrieval   |
| `/api/auth`          | GET, POST | Windows Auth | Authentication operations   |
| `/api/external`      | GET, POST | API Key      | External system integration |
| `/api/scheduled/run` | POST      | Windows Auth | Trigger scheduled tasks     |
| `/health`            | GET       | Anonymous    | Health check endpoint       |

## Configuration

### Application Settings

Configuration is managed through `appsettings.json`:

```json
{
  "AppConfig": {
    "ApplicationName": "MyApp",
    "MaxPageSize": 100,
    "DefaultPageSize": 20,
    "EnableDetailedErrors": false,
    "ExternalApiBaseUrl": "https://api.external.example.com",
    "ExternalApiTimeout": 30,
    "AllowedApiKeyHashes": []
  }
}
```

### API Key Authentication

For external endpoints, API keys are validated using DPAPI-protected hashes:

```powershell
# Generate protected API key hash
./tests/Scripts/Protect-ApiKey.ps1 -ApiKey "your-api-key"
```

Add the generated hash to `AllowedApiKeyHashes` in configuration.

## Deployment

### IIS Deployment

1. Install ASP.NET Core Hosting Bundle
2. Run MSI installer
3. Configure application pool identity
4. Set up Windows Authentication in IIS
5. Configure HTTPS bindings

### Configuration for Production

1. Update `appsettings.Production.json` with production values
2. Configure DPAPI-protected API keys
3. Set up logging to Windows Event Log
4. Configure health check monitoring

## Security

- **Windows Authentication** - Uses Negotiate (Kerberos/NTLM)
- **API Key Auth** - DPAPI-protected hashes for external access
- **HTTPS Only** - Enforced in production
- **Request Logging** - Audit trail for all requests

## License

MIT License - See [LICENSE](LICENSE) for details.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make changes following [Conventional Commits](https://www.conventionalcommits.org/)
4. Run tests and linting (`dotnet test && dotnet format --verify-no-changes`)
5. Submit a pull request
6. Ensure CI passes

## Support

For issues and feature requests, please use [GitHub Issues](https://github.com/thpham/gha-dotnet-rflow/issues).
