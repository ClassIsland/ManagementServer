import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { LaptopOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'get_started';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/get_started',
    name: routeName,
    component: () => import('@/views/auth/AuthViewBase.vue'),
    meta: {
      title: '开始使用',
      hidden: true,
      roles: ["Admin"]
    },
    redirect: "/get_started/wizard",
    children: [
      {
        path: 'wizard',
        name: routeName + "_main",
        component: () => import('@/views/get_started/index.vue'),
        meta: {
          title: '开始使用',
          hidden: true,
          overrideDefaultAuthContainer: true,
          width: 540
        }
      },
      {
        path: 'completed',
        name: routeName + "_completed",
        component: () => import('@/views/get_started/completed.vue'),
        meta: {
          title: '完成',
          hidden: true,
          overrideDefaultAuthContainer: true,
          width: 540
        }
      }
    ]
  }
]

export default routes;
