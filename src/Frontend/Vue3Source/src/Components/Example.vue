<template>
  <h1>{{ title }}</h1>
  <p>{{ count }}</p>
  <button @click="count++">Increment</button>
</template>

<script lang="ts">
import { defineComponent} from 'vue'
export default defineComponent({
  name: 'Example',
  props: {
    title: { type: String, default: '' },
  },
    data() {
    return { count: 0 };
  },
})
</script>