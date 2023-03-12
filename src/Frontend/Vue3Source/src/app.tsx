import { createSSRApp, defineComponent, h  } from 'vue'
import { renderToString } from 'vue/server-renderer'
import Example from './Components/Example.vue'

  // const app = createSSRApp(Example, {title: "Vue3 SSR in .Net"})

globalThis.defineComponent = defineComponent;
globalThis.h = h;
globalThis.Components = {Example};
globalThis.createSSRApp = createSSRApp;
globalThis.renderToString = renderToString;

