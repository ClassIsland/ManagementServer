import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { ClusterOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/policies',
    name: 'policies',
    redirect: '/policies/list',
    component: Layout,
    meta: {
      title: '策略',
      icon: renderIcon(ClusterOutlined),
      sort: 5,
    },
    children: [
      {
        path: 'list',
        name: 'policies_list',
        meta: {
          title: '策略',
        },
        component: () => import('@/views/policies/index.vue'),
      }
    ]
  },
];

export default routes;
