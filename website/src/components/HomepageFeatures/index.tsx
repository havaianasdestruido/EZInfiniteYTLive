import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

type FeatureItem = {
  title: string;
  emoji: string;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    title: 'Folder-Based Infinite Loop',
    emoji: '🔁',
    description: (
      <>
        Point EZInfiniteYTLive at a folder of video files and it streams them
        back-to-back, forever, restarting from the top once the folder has
        played through — no playlist files required.
      </>
    ),
  },
  {
    title: 'Any RTMP Destination',
    emoji: '📡',
    description: (
      <>
        Works with YouTube Live out of the box, and with any other
        RTMP-compatible service — Twitch, Kick, or a self-hosted server —
        just change the RTMP URL and stream key.
      </>
    ),
  },
  {
    title: 'Lightweight & FFmpeg-Powered',
    emoji: '🪶',
    description: (
      <>
        A minimal WinForms GUI wrapping FFmpeg with stream-copy (no
        re-encoding), keeping CPU usage low enough to run 24/7 on modest
        hardware.
      </>
    ),
  },
];

function Feature({title, emoji, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
        <span className={styles.featureEmoji} role="img" aria-hidden="true">
          {emoji}
        </span>
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
