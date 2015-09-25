FROM centos:latest
MAINTAINER Henrik Feldt <henrik@haf.se>

ENV BUILD_PREFIX /tmp/build
RUN mkdir -p $BUILD_PREFIX && \
    mkdir -p $BUILD_PREFIX/tesseract.net

WORKDIR $BUILD_PREFIX

# common packages across builds:
RUN yum install -y epel-release yum-utils && \
    rpm --import "http://keyserver.ubuntu.com/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF" && \
    yum-config-manager --add-repo http://download.mono-project.com/repo/centos/ && \
    yum install -y mono-complete \
                   python-setuptools hostname inotify-tools yum-utils ruby-devel \
                   gcc gcc-c++ rpm-build redhat-rpm-config make readline-devel \
                   tar openssl-devel zlib-devel libffi-devel sqlite-devel \
                   git libtool automake autoconf \
                   # requirement for liblept:
                   libpng12-devel libjpeg-turbo-devel libwebp-devel

# build the libraries
RUN git clone https://github.com/haf/leptonica.git && \
    git clone https://github.com/haf/tesseract.git

WORKDIR $BUILD_PREFIX/leptonica

RUN ./configure && \
    make && \
    make install

WORKDIR $BUILD_PREFIX/tesseract

RUN ./autogen.sh && \
    ./configure && \
    make && \
    make install

RUN cp -r /usr/local/lib/* /usr/lib && ldconfig -n -v /usr/lib
# OR:
# ENV LD_LIBRARY_PATH /usr/lib:/usr/local/lib

WORKDIR $BUILD_PREFIX/tesseract.net

RUN gem install bundler

COPY . $BUILD_PREFIX/tesseract.net

RUN bundle
RUN bundle exec rake

ENTRYPOINT ["/bin/bash"]