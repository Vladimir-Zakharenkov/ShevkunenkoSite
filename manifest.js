const manifest = {
    name: 'My App',
    short_name: 'My App',
    display: 'standalone',
    theme_color: '#ffffff',
    background_color: '#ffffff',
    icons: [
        { src: `${window.location.origin}/path/to/icon.png`, sizes: '192x192', type: 'image/png' },
        { src: `${window.location.origin}/path/to/icon.png`, sizes: '512x512', type: 'image/png' },
        { src: `${window.location.origin}/path/to/icon.png`, sizes: '192x192', type: 'image/png', purpose: 'maskable' },
        { src: `${window.location.origin}/path/to/icon.png`, sizes: '512x512', type: 'image/png', purpose: 'maskable' }
    ],
    start_url: window.location.href
};

const link = document.createElement('link');
link.rel = 'manifest';
link.href = `data:application/json;base64,${btoa(JSON.stringify(manifest))}`;
document.head.appendChild(link);