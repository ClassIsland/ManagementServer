import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { SettingOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/setting',
    name: 'Setting',
    redirect: '/setting/account',
    component: Layout,
    meta: {
      title: '设置',
      icon: renderIcon(SettingOutlined),
      sort: 5,
    },
    children: [
      {
        path: 'account',
        name: 'setting-account',
        meta: {
          title: '个人设置',
        },
        component: () => import('@/views/setting/account/account.vue'),
      },
      {
        path: 'system',
        name: 'setting-system',
        meta: {
          title: '系统设置',
          roles: ['Admin']
        },
        component: () => import('@/views/setting/system/system.vue'),
      },
      {
        path: 'users',
        name: 'setting-users',
        meta: {
          title: '用户管理',
          roles: ['UsersManager']
        },
        component: () => import('@/views/setting/users/index.vue'),
      },
    ],
  },
];

export default routes;
