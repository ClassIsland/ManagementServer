import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { ThunderboltOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'automation';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/automation',
    name: routeName,
    component: Layout,
    meta: {
      title: '自动化',
      sort: 94,
      icon: renderIcon(ThunderboltOutlined),
    },
    children: [
      {
        path: 'management',
        name: `${routeName}_management`,
        component: () => import('@/views/automation/index.vue'),
        meta: {
          title: '自动化管理',
          icon: renderIcon(ThunderboltOutlined),
        },
      },
    ],
  },
];

export default routes;
