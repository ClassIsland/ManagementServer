import { createApis, withConfigType } from './createApis';
import {Alova} from "@/utils/http/alova";

export const $$userConfigMap = withConfigType({});

const Apis = createApis(Alova, $$userConfigMap);

export default Apis;
