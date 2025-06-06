import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { LaptopOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'clients';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/clients',
    name: routeName,
    redirect: '/clients/list',
    component: Layout,
    meta: {
      title: '实例',
      icon: renderIcon(LaptopOutlined),
      permissions: ['client_manage'],
      sort: 1,
    },
    children: [
      {
        path: '/clients/list',
        name: `${routeName}_list`,
        meta: {
          title: '实例列表',
        },
        component: () => import('@/views/clients/list/index.vue'),
      },
      {
        path: '/clients/groups',
        name: `${routeName}_groups`,
        meta: {
          title: '分组',
        },
        component: () => import('@/views/clients/groups/index.vue'),
      },
    ],
  },
];

export default routes;
