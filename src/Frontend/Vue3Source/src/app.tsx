import { createSSRApp, defineComponent, h , createApp } from 'vue'
import { renderToString } from 'vue/server-renderer'
import Example from './Components/Example.vue'

globalThis.defineComponent = defineComponent;
globalThis.h = h;
globalThis.Components = {Example};
globalThis.createSSRApp = createSSRApp;
globalThis.renderToString = renderToString;
globalThis.createApp = createApp;
