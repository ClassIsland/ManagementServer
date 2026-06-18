import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { AppstoreOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'components';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/components',
    name: routeName,
    component: Layout,
    meta: {
      title: '组件配置',
      sort: 95,
      icon: renderIcon(AppstoreOutlined),
    },
    children: [
      {
        path: 'management',
        name: `${routeName}_management`,
        component: () => import('@/views/components/index.vue'),
        meta: {
          title: '组件管理',
          icon: renderIcon(AppstoreOutlined),
        },
      },
    ],
  },
];

export default routes;
