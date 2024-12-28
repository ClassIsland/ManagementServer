import { RouteRecordRaw } from 'vue-router';
import { Layout } from '@/router/constant';
import { FileOutlined, BookOutlined, TableOutlined, CalendarOutlined, UploadOutlined } from '@vicons/antd';
import { renderIcon } from '@/utils/index';

const routeName = 'profiles';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/profiles',
    name: routeName,
    component: Layout,
    meta: {
      title: '档案',
      icon: renderIcon(FileOutlined),
      sort: 1,
    },
    children: [
      {
        path: '/profiles/classplans',
        name: `${routeName}_classPlans`,
        meta: {
          title: '课表',
          icon: renderIcon(TableOutlined),
        },
        component: () => import('@/views/profiles/subjects/index.vue'),
      },
      {
        path: '/profiles/timetables',
        name: `${routeName}_timetables`,
        meta: {
          title: '时间表',
          icon: renderIcon(CalendarOutlined),
        },
        component: () => import('@/views/profiles/subjects/index.vue'),
      },
      {
        path: '/profiles/subjects',
        name: `${routeName}_subjects`,
        meta: {
          title: '科目',
          icon: renderIcon(BookOutlined),
        },
        component: () => import('@/views/profiles/subjects/index.vue'),
      },
      {
        path: '/profiles/upload',
        name: `${routeName}_upload`,
        meta: {
          title: '上传',
          icon: renderIcon(UploadOutlined),
        },
        component: () => import('@/views/profiles/upload/index.vue'),
      },
      
    ],
  },
];

export default routes;
