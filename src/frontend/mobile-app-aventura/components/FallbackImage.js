import React, { useState } from 'react';
import { Image } from 'react-native';

const DEFAULT_FALLBACK = require('../assets/home/amazonica.jpeg');

/**
 * Wrapper de <Image> que troca automaticamente para uma imagem local de fallback
 * quando o carregamento da imagem remota falha (ex.: espetáculo sem imagem
 * cadastrada no Mongo, backend fora do ar, rede instável). Usado nos lugares
 * que exibem a imagem de um espetáculo vinda de getShowImage()/getShowImageUrl().
 */
export default function FallbackImage({ source, fallbackSource = DEFAULT_FALLBACK, ...props }) {
  const [hasError, setHasError] = useState(false);

  const isRemote = Boolean(source) && typeof source === 'object' && 'uri' in source;

  let resolvedSource = source;
  if (isRemote && hasError) {
    resolvedSource = fallbackSource;
  }

  return (
    <Image
      {...props}
      source={resolvedSource}
      onError={isRemote ? () => setHasError(true) : props.onError}
    />
  );
}