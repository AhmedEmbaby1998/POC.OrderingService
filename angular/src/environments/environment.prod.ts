import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44318/',
  redirectUri: baseUrl,
  clientId: 'POC_App',
  responseType: 'code',
  scope: 'offline_access POC',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'POC',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44318',
      rootNamespace: 'POC',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
