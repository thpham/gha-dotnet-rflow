# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0](https://github.com/thpham/gha-dotnet-rflow/compare/v1.0.0...v1.1.0) (2026-01-09)


### Features

* **ci:** add automatic backport label suggestions for fix/security PRs ([aa64188](https://github.com/thpham/gha-dotnet-rflow/commit/aa64188ee647884040ab697c0c8b61a71d82645a))
* **ci:** enhance CI/CD pipeline with security best practices ([a5586e7](https://github.com/thpham/gha-dotnet-rflow/commit/a5586e7a1df66f314bbd1661c08b9c3b71a27809))
* **ci:** support self-hosted SonarQube in addition to SonarCloud ([49d4d36](https://github.com/thpham/gha-dotnet-rflow/commit/49d4d36f2d63675a38d9e477ca74fbcb4b909643))
* **enrollment:** add Windows Certificate Enrollment Services demo ([9db6c00](https://github.com/thpham/gha-dotnet-rflow/commit/9db6c0001b497f0dd0208de3feff7d587ed33d46))
* initial Windows .NET 9 application with CI/CD pipeline ([a7c9951](https://github.com/thpham/gha-dotnet-rflow/commit/a7c9951c2869ab130a0407b22a1eb8ffe97e736b))


### Bug Fixes

* **app:** resolve ambiguous TaskStatus type reference ([7021cd4](https://github.com/thpham/gha-dotnet-rflow/commit/7021cd49e868273fb9e2842104791291beda83b4))
* **ci:** consolidate Pester test steps to fix process lifecycle issues ([017865f](https://github.com/thpham/gha-dotnet-rflow/commit/017865f79e6605da7be88b9429155a58c03478cc))
* **ci:** enable real Pester integration tests with app startup ([c410503](https://github.com/thpham/gha-dotnet-rflow/commit/c41050379aab69ce5206435a3e096243202a914f))
* **ci:** use plain HTTP for Pester integration tests ([9b46459](https://github.com/thpham/gha-dotnet-rflow/commit/9b46459eb80e900fc867a85bf1651fd15c8277c0))
* **tests:** add missing Xunit import and fix AllSatisfy assertion ([d7ff123](https://github.com/thpham/gha-dotnet-rflow/commit/d7ff1231873fcc0296366a090a8985458e93615e))
* **tests:** exclude integration tests from CI pipeline ([d66ef58](https://github.com/thpham/gha-dotnet-rflow/commit/d66ef581c4a16e8b6b5575cbe03e42568f8a8636))


### Performance Improvements

* **ci:** add NuGet cache restore to lint job ([221ad45](https://github.com/thpham/gha-dotnet-rflow/commit/221ad45113291ab140507a12a6da163daa880955))
* **ci:** optimize lint job by skipping restore ([53178a2](https://github.com/thpham/gha-dotnet-rflow/commit/53178a2575138dd08a2b8d40fc6257ea03ff8b57))
* **ci:** reuse build artifact for Pester integration tests ([11063d1](https://github.com/thpham/gha-dotnet-rflow/commit/11063d1ec95429e38136785752b899c9c5702a11))
* **ci:** use inclusion-based path filters for CI triggers ([a6aa552](https://github.com/thpham/gha-dotnet-rflow/commit/a6aa552f65b22556fd102c1e2f3bb3a3c48daf07))


### Documentation

* add XML documentation to CertEnrollRequestOptions properties ([babc5cb](https://github.com/thpham/gha-dotnet-rflow/commit/babc5cb728426ce0c446ac74698871800852c617))


### Build System

* **deps:** bump dependencies from Dependabot PRs ([55488ce](https://github.com/thpham/gha-dotnet-rflow/commit/55488ce038a1e58c396bcdc8bbfeb6283e242952))

## [Unreleased]

### Added

- Initial project structure
- ASP.NET Core Web API with SOAP-style and REST endpoints
- Windows Authentication support
- Anonymous access for external system integration
- Scheduled task management API
- WiX v4 MSI installer with IIS configuration
- GitHub Actions CI/CD pipeline
- Release Please automation
- dotnet-releaser integration
- Pester PowerShell tests
- xUnit unit and integration tests

### Endpoints

- `GET/POST /api/data` - SOAP-style GetData endpoint (Windows Auth)
- `GET/POST /api/auth` - Authentication operations (Windows Auth)
- `GET/POST /api/external` - External system integration (API Key Auth)
- `POST /api/scheduled/run` - Scheduled task trigger (Windows Auth)
- `GET /health` - Health check endpoint

## [1.0.0] - TBD

Initial release.

[Unreleased]: https://github.com/thpham/gha-dotnet-rflow/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/thpham/gha-dotnet-rflow/releases/tag/v1.0.0
