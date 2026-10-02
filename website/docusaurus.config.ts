import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
  title: 'EZInfiniteYTLive',
  tagline: 'Easy 24/7 looping RTMP livestreams, powered by FFmpeg',
  favicon: 'img/favicon.ico',

  // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  // Set the production url of your site here
  url: 'https://havaianasdestruido.github.io',
  // Set the /<baseUrl>/ pathname under which your site is served.
  // Using '/' keeps local dev / preview environments simple; if you deploy
  // to GitHub Pages under https://<org>.github.io/EZInfiniteYTLive/, change
  // this to '/EZInfiniteYTLive/' (and update links accordingly).
  baseUrl: '/',

  // GitHub pages deployment config.
  organizationName: 'havaianasdestruido', // Usually your GitHub org/user name.
  projectName: 'EZInfiniteYTLive', // Usually your repo name.

  onBrokenLinks: 'throw',

  markdown: {
    mermaid: true,
  },
  themes: ['@docusaurus/theme-mermaid'],

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          editUrl:
            'https://github.com/havaianasdestruido/EZInfiniteYTLive/tree/main/website/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    // Replace with your project's social card
    image: 'img/logo.svg',
    colorMode: {
      respectPrefersColorScheme: true,
    },
    navbar: {
      title: 'EZInfiniteYTLive',
      logo: {
        alt: 'EZInfiniteYTLive Logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'tutorialSidebar',
          position: 'left',
          label: 'Documentation',
        },
        {
          href: 'https://github.com/havaianasdestruido/EZInfiniteYTLive',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Docs',
          items: [
            {
              label: 'Introduction',
              to: '/docs/intro',
            },
            {
              label: 'Getting Started',
              to: '/docs/getting-started/installation',
            },
            {
              label: 'API Reference',
              to: '/docs/api-reference/video-streamer',
            },
          ],
        },
        {
          title: 'Project',
          items: [
            {
              label: 'GitHub Repository',
              href: 'https://github.com/havaianasdestruido/EZInfiniteYTLive',
            },
            {
              label: 'Issues',
              href: 'https://github.com/havaianasdestruido/EZInfiniteYTLive/issues',
            },
            {
              label: 'License',
              href: 'https://github.com/havaianasdestruido/EZInfiniteYTLive/blob/main/LICENSE.txt',
            },
          ],
        },
        {
          title: 'More',
          items: [
            {
              label: 'FFmpeg',
              href: 'https://ffmpeg.org/',
            },
            {
              label: 'Docusaurus',
              href: 'https://docusaurus.io/',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} EZInfiniteYTLive. Documentation built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ['csharp', 'powershell', 'bash'],
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
