FROM sebp/elk:661
MAINTAINER "Vladimir Vitkov <v.vitkov@is-bg.net>"

# install ES plugins
ENV ES_HOME /opt/elasticsearch
WORKDIR ${ES_HOME}

RUN yes | CONF_DIR=/etc/elasticsearch gosu elasticsearch bin/elasticsearch-plugin install -b ingest-attachment

# install LS plugins
#ENV LOGSTASH_HOME /opt/logstash
#WORKDIR ${LOGSTASH_HOME}
#
#RUN gosu logstash bin/logstash-plugin install logstash-input-rss