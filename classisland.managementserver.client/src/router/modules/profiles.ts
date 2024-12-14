import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { DashboardOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'profiles';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/profiles',
    name: routeName,
    redirect: '/profiles/',
    component: Layout,
    meta: {
      title: '档案',
      icon: renderIcon(DashboardOutlined),
      permissions: ['profile_manage'],
      sort: 1,
    },
    children: [
      {
        path: '/profiles/subjects',
        name: `${routeName}_subjects`,
        meta: {
          title: '科目',
        },
        component: () => import('@/views/profiles/subjects/index.vue'),
      },
      
    ],
  },
];

export default routes;
