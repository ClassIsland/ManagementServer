<script lang="ts">
  import { defineComponent, onBeforeMount, h } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { NEmpty } from 'naive-ui';

  export default defineComponent({
    name: 'Redirect',
    setup() {
      const route = useRoute();
      const router = useRouter();
      onBeforeMount(() => {
        const { params, query } = route;
        const { path } = params;
        router.replace({
          path: '/' + (Array.isArray(path) ? path.join('/') : path),
          query,
        });
      });
      return () => h(NEmpty);
    },
  });
</script>
