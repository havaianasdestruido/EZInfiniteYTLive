---
layout: default
title: Simple, infinite RTMP livestreams
---

<section class="hero">
  <div class="container">
    <p class="eyebrow">FFmpeg-powered Windows desktop app</p>
    <h1>Keep your livestream running, forever.</h1>
    <p class="hero__lead">EZInfiniteYTLive turns a folder of videos into a continuous 24/7 livestream for YouTube Live or any RTMP-compatible service.</p>
    <div class="actions">
      <a class="button" href="{{ '/docs/' | relative_url }}">Read the documentation</a>
      <a class="button button--secondary" href="https://github.com/havaianasdestruido/EZInfiniteYTLive">View on GitHub</a>
    </div>
  </div>
</section>

<section class="section">
  <div class="container">
    <h2>Everything you need for an always-on stream</h2>
    <p class="section__intro">Choose a folder, enter your RTMP destination, and let the lightweight WinForms app handle the playlist loop through FFmpeg.</p>
    <div class="feature-grid">
      <article class="feature">
        <span class="feature__icon" aria-hidden="true">🔁</span>
        <h3>Infinite looping</h3>
        <p>Videos play back-to-back and restart from the beginning when the folder reaches its end.</p>
      </article>
      <article class="feature">
        <span class="feature__icon" aria-hidden="true">📡</span>
        <h3>Any RTMP destination</h3>
        <p>Stream to YouTube, Twitch, Kick, or a self-hosted RTMP server with a URL and stream key.</p>
      </article>
      <article class="feature">
        <span class="feature__icon" aria-hidden="true">🪶</span>
        <h3>Lightweight by design</h3>
        <p>FFmpeg remuxes compatible source files without re-encoding, keeping CPU usage low.</p>
      </article>
    </div>
  </div>
</section>

<section class="section section--tinted">
  <div class="container">
    <h2>Start in three steps</h2>
    <div class="workflow">
      <div class="workflow__step">
        <span class="workflow__number">1</span>
        <div><h3>Choose videos</h3><p>Select the folder containing your <code>.mp4</code>, <code>.mkv</code>, <code>.avi</code>, <code>.mov</code>, or <code>.flv</code> files.</p></div>
      </div>
      <div class="workflow__step">
        <span class="workflow__number">2</span>
        <div><h3>Set the destination</h3><p>Enter an RTMP URL and stream key, then choose alphabetic or random playback order.</p></div>
      </div>
      <div class="workflow__step">
        <span class="workflow__number">3</span>
        <div><h3>Press START</h3><p>FFmpeg sends each file to your live endpoint until you stop the stream.</p></div>
      </div>
    </div>
  </div>
</section>
