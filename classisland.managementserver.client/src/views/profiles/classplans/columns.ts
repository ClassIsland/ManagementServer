import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {ClassPlan, Client, Subject} from "@/api/globals";

export const columns: BasicColumn<ClassPlan>[] = [
  {
    title: '名称',
    key: 'name',
  },
  {
    title: '时间表',
    key: 'timeLayout'
  }
];
