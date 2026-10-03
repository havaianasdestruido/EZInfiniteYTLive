# EZInfiniteYTLive

## Star History

<a href="https://www.star-history.com/?repos=havaianasdestruido%2FEZInfiniteYTLive&type=date&legend=top-left">
 <picture>
   <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/chart?repos=havaianasdestruido/EZInfiniteYTLive&type=date&theme=dark&legend=top-left" />
   <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/chart?repos=havaianasdestruido/EZInfiniteYTLive&type=date&legend=top-left" />
   <img alt="Star History Chart" src="https://api.star-history.com/chart?repos=havaianasdestruido/EZInfiniteYTLive&type=date&legend=top-left" />
 </picture>
</a>

Easy to use tool to make 24/7 livestreams, supporting even non-YouTube RMTP services.

Only requires FFMPEG to be installed (on PATH, but i guess it does that by default too).
<img width="513" height="137" alt="image" src="https://github.com/user-attachments/assets/5bd24a48-c353-47a0-8ef9-0e1af8d9a16b" />

## How to use

`Choose videos` -> select your video folder

`RandOrder?` -> random order.

`AlphabeticOrder?` -> alphabetic order. (considering each file name)

## Documentation

The primary project webpage is built with Jekyll and is published at:

<https://havaianasdestruido.github.io/EZInfiniteYTLive/>

Full codebase documentation (architecture, usage guide, API reference, configuration, CI/CD, and contributing guide) is available at [`/docs`](https://havaianasdestruido.github.io/EZInfiniteYTLive/docs/) and in the [`website/`](website) directory. GitHub Pages builds the Jekyll site first, then mounts the Docusaurus output under `/docs` so the two site generators do not compete for the root page.

To run the documentation locally:

```bash
cd website
npm install
npm start
```

Then open http://localhost:3000 in your browser. See [`website/docs/index.md`](website/docs/index.md) to start reading without running the site.
