FROM centos:latest
MAINTAINER Henrik Feldt <henrik@haf.se>

ENV BUILD_PREFIX /tmp/build
ENV APP_PREFIX /app
ENV LISTEN_PORT=8083

RUN mkdir -p $BUILD_PREFIX

WORKDIR $BUILD_PREFIX

# common packages across builds:
RUN yum install -y epel-release yum-utils && \
    rpm --import "http://keyserver.ubuntu.com/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF" && \
    yum-config-manager --add-repo http://download.mono-project.com/repo/centos/ && \
    yum install -y mono-complete
RUN  yum install -y python-setuptools hostname inotify-tools yum-utils ruby-devel \
                 gcc gcc-c++ rpm-build redhat-rpm-config make readline-devel \
                 tar openssl-devel zlib-devel libffi-devel sqlite-devel

RUN gem install bundler

COPY . $BUILD_PREFIX

RUN bundle
RUN bundle exec rake


RUN gem install rake albacore
RUN rake

ENTRYPOINT ["/bin/bash"]