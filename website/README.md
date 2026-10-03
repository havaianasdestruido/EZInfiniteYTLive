# Documentation site

The project homepage at the repository root is built with Jekyll. This directory contains the Docusaurus documentation site, which GitHub Pages publishes only under [`/docs`](https://havaianasdestruido.github.io/EZInfiniteYTLive/docs/).

## Installation

```bash
npm install
```

**Note**: feel free to use the package manager of your choice.

## Local Development

```bash
npm run start
```

This command starts a local development server and opens up a browser window. Most changes are reflected live without having to restart the server.

## Build

```bash
npm run build
```

This command generates the documentation into the `build` directory and can be served using any static content host. The GitHub Pages workflow sets `DEPLOY_ENV=github-pages` so asset links point to `/EZInfiniteYTLive/docs/` before mounting this build at that path.

## Deployment

Deployment is handled by [`.github/workflows/pages.yml`](../.github/workflows/pages.yml). It builds the repository root with Jekyll, builds this site with Docusaurus, and publishes this `build` directory below `/docs`; no `docusaurus deploy` or `gh-pages` branch is needed.
